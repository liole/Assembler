using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	interface IInstruction
	{
		int LineNumber { get; }
		byte[] Assemble(MemoryManager mgr);
	}
}
