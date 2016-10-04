using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	class Label: IInstruction
	{
		public string Name { get; set; }

		public Label(string name)
		{
			Name = name;
		}

		public byte[] Assemble(MemoryManager mgr)
		{
			throw new NotImplementedException();
		}
	}
}
