using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	class Definition: IInstruction
	{
		public string Name { get; set; }

		public byte[] Assemble(MemoryManager mgr)
		{
			throw new NotImplementedException();
		}
	}
}
