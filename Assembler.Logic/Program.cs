using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	public class Program
	{
		public event EventHandler<Exceptions.ExceptionHandlerArgs> ExceptionHandler;

		private List<IInstruction> program;
		private MemoryManager memoryManager;

		private List<byte>[] codeLines;

		public Program()
		{
			program = new List<IInstruction>();
			memoryManager = new MemoryManager();
		}

		internal void Add(IInstruction instruction)
		{
			program.Add(instruction);
		}

		internal void Add(Definition definition)
		{
			Add(definition as IInstruction);
			if (definition.Name != null)
			{
				if (memoryManager.IsVariableDecalared(definition.Name))
				{
					throw new Exceptions.VariableRedeclaredException(definition.Name);
				}
				memoryManager.DeclareVariable(definition.Name, definition.Type);
			}
		}

		internal void Add(Label label)
		{
			Add(label as IInstruction);
			if (memoryManager.IsLabelDecalared(label.Name))
			{
				throw new Exceptions.LabelRedeclaredException(label.Name);
			}
			memoryManager.DeclareLabel(label.Name);
		}

		internal void Add(Procedure procedure)
		{
			Add(procedure as IInstruction);
			if (!procedure.End)
			{
				if (memoryManager.IsProcedureDecalared(procedure.Name))
				{
					throw new Exceptions.ProcedureRedeclaredException(procedure.Name);
				}
				memoryManager.DeclareProcedure(procedure.Name);
			}
			else
			{
				if (!memoryManager.IsProcedureDecalared(procedure.Name))
				{
					throw new Exceptions.ProcedureNotDeclaredException(procedure.Name, "ENDP");
				}
				if (memoryManager.IsProcedureEnded(procedure.Name))
				{
					throw new Exceptions.ProcedureAlreadyEndedException(procedure.Name);
				}
				memoryManager.EndProcedureDeclaration(procedure.Name);
			}
		}

		private void initCodeLines()
		{
			int lineCount = program.Count == 0 ? 0 : (program.Last().LineNumber + 1);
			codeLines = new List<byte>[lineCount];
			for (int i = 0; i < codeLines.Length; ++i)
			{
				codeLines[i] = new List<byte>();
			}
		}

		public byte[] Assemble()
		{
			memoryManager.ResetPointer();
			initCodeLines();
			foreach(var instruction in program)
			{
				try
				{
					var instrCode = instruction.Assemble(memoryManager);
					memoryManager.MovePointer((Int16)instrCode.Length);
					int n = instruction.LineNumber;
					codeLines[n].AddRange(instrCode);
				}
				catch(Exceptions.LineException e)
				{
					if (e.CaptureInfo == null)
					{
						e.CaptureInfo = new Lexer.CaptureInfo()
						{
							LineNumber = instruction.LineNumber
						};
					}
					program_ExceptionHandler(e);
				}
			}
			return Code;
		}

		public byte[][] CodeLines
		{
			get
			{
				if (codeLines == null)
				{
					throw new Exception("Assemble first");
				}
				return codeLines.Select(line => line.ToArray()).ToArray();
			}
		}

		public byte[] Code
		{
			get
			{
				if (codeLines == null)
				{
					throw new Exception("Assemble first");
				}
				return codeLines.SelectMany(line => line).ToArray();
			}
		}

		public class ProcedureLineInfo
		{
			public string Name { get; set; }
			public int Start { get; set; }
			public int End { get; set; }
		}
		public ProcedureLineInfo[] ProcedureLines
		{
			get
			{
				var procedures = program.OfType<Procedure>();
				var res = new List<ProcedureLineInfo>();
				foreach(var proc in procedures)
				{
					if (!proc.End)
					{
						var end = procedures.FirstOrDefault(p => p.Name == proc.Name && p.End);
						if (end != null)
						{
							res.Add(new ProcedureLineInfo()
							{
								Name = proc.Name,
								Start = proc.LineNumber,
								End = end.LineNumber
							});
						}
					}
				}
				return res.ToArray();
			}
		}

		public class VariableInfo
		{
			public string Name { get; set; }
			public DefinitionType Type { get; set; }
			public int Line { get; set; }
			public Int16 Address { get; set; }
		}
		public VariableInfo[] VariablesInfo
		{
			get
			{
				var result = new List<VariableInfo>();
				var variables = program.OfType<Definition>();
				foreach (var name in memoryManager.Variables.Keys)
				{
					result.Add(new VariableInfo()
						{
							Name = name,
							Type = memoryManager.VariableTypes[name],
							Address = memoryManager.Variables[name],
							Line = variables.First(d => d.Name == name).LineNumber + 1
						});
				}
				return result.ToArray();
			}
		}

		public class LabelInfo
		{
			public string Name { get; set; }
			public int Line { get; set; }
			public Int16 Address { get; set; }
		}
		public LabelInfo[] LabelsInfo
		{
			get
			{
				var result = new List<LabelInfo>();
				var labels = program.OfType<Label>();
				foreach (var name in memoryManager.Labels.Keys)
				{
					result.Add(new LabelInfo()
					{
						Name = name,
						Address = memoryManager.Labels[name],
						Line = labels.First(l => l.Name == name).LineNumber + 1
					});
				}
				return result.ToArray();
			}
		}

		public class ProcedureInfo
		{
			public string Name { get; set; }
			public int Line { get; set; }
			public Int16 Address { get; set; }
		}
		public ProcedureInfo[] ProceduresInfo
		{
			get
			{
				var result = new List<ProcedureInfo>();
				var procedures = program.OfType<Procedure>();
				foreach (var name in memoryManager.Procedures.Keys)
				{
					result.Add(new ProcedureInfo()
					{
						Name = name,
						Address = memoryManager.Procedures[name],
						Line = procedures.First(p => p.Name == name).LineNumber + 1
					});
				}
				return result.ToArray();
			}
		}

		public int SymbolsCount
		{
			get
			{
				var variables = memoryManager.Variables.Count;
				var labels = memoryManager.Labels.Count;
				var procedures = memoryManager.Procedures.Count;
				return variables + labels + procedures;
			}
		}

		private void program_ExceptionHandler(Exceptions.LineException e)
		{
			if (ExceptionHandler != null)
			{
				ExceptionHandler(this, new Exceptions.ExceptionHandlerArgs(e));
			}
			else
			{
				throw e;
			}
		}
	}
}
