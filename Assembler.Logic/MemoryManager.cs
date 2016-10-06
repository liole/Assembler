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
		public Dictionary<string, Definition.DefinitionType> VariableTypes { get; private set; }

		public static Int16 UndefinedAddress = -1; //0xffff;

		public MemoryManager()
		{
			Pointer = 0;
			Labels = new Dictionary<string, Int16>();
			Variables = new Dictionary<string, Int16>();
			VariableTypes = new Dictionary<string, Definition.DefinitionType>();
		}

		public Int16 MovePointer(Int16 offset)
		{
			Pointer += offset;
			return Pointer;
		}

		public Int16 ResetPointer(Int16 value = 0)
		{
			Pointer = value;
			return Pointer;
		}

		public void DeclareLabel(string name)
		{
			Labels[name] = UndefinedAddress;
		}

		public void DeclareVariable(string name, Definition.DefinitionType type)
		{
			Variables[name] = UndefinedAddress;
			VariableTypes[name] = type;
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
