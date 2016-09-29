using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	class Program
	{
		private List<Instruction> program;
		private MemoryManager memoryManager;

		public Program()
		{
			program = new List<Instruction>();
			memoryManager = new MemoryManager();
		}

		public void Add(Instruction instruction)
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
	}
}
