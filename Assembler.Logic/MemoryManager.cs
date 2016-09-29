using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	class MemoryManager
	{
		public Int16 Pointer { get; private set; }
		public Dictionary<string, Int16> Labels { get; private set; }
		public Dictionary<string, Int16> Variables { get; private set; }

		public MemoryManager()
		{
			Pointer = 0;
		}

		public Int16 MovePointer(Int16 offset)
		{
			Pointer += offset;
			return Pointer;
		}

		public void DeclareLabel(string name)
		{
			Labels[name] = 0;
		}

		public void DeclareVariable(string name)
		{
			Variables[name] = 0;
		}

		public Int16 DefineLabel(string name)
		{
			Labels[name] = Pointer;
			return Pointer;
		}

		public Int16 DefineVariable(string name)
		{
			Variables[name] = Pointer;
			return Pointer;
		}
	}
}
