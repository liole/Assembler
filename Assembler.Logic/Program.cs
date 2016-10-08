using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	public class Program
	{
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
			if (instruction is Definition)
			{
				var definition = instruction as Definition;
				if (definition.Name != null)
				{
					if (memoryManager.Variables.ContainsKey(definition.Name))
					{
						throw new Exceptions.VariableRedeclaredException(definition.LineNumber, definition.Name);
					}
					memoryManager.DeclareVariable(definition.Name, definition.Type);
				}
			}
			if (instruction is Label)
			{
				var label = instruction as Label;
				memoryManager.DeclareLabel(label.Name);
			}
		}

		private void initCodeLines()
		{
			int lineCount = program.Last().LineNumber;
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
				var instrCode = instruction.Assemble(memoryManager);
				memoryManager.MovePointer((Int16)instrCode.Length);
				int n = instruction.LineNumber;
				codeLines[n - 1].AddRange(instrCode);
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
	}
}
