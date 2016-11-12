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
