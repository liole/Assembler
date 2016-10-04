using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	class Program
	{
		private List<IInstruction> program;
		private MemoryManager memoryManager;

		public Program()
		{
			program = new List<IInstruction>();
			memoryManager = new MemoryManager();
		}

		public void Add(IInstruction instruction)
		{
			program.Add(instruction);
			if (instruction is Definition)
			{
				var definition = instruction as Definition;
				if (definition.Name != null)
				{
					memoryManager.DeclareVariable(definition.Name);
				}
			}
			if (instruction is Label)
			{
				var label = instruction as Label;
				memoryManager.DeclareLabel(label.Name);
			}
		}

		public byte[] Assemble()
		{
			memoryManager.ResetPointer();
			var code = new List<byte>();
			foreach(var instruction in program)
			{
				var instrCode = instruction.Assemble(memoryManager);
				memoryManager.MovePointer((Int16)instrCode.Length);
				code.AddRange(instrCode);
			}
			return code.ToArray();
		}
	}
}
