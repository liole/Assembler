using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Commands
{
	class Command: IInstruction
	{
		public Func<MemoryManager, byte[]> Assemble { get; set; }

		byte[] IInstruction.Assemble(MemoryManager mgr)
		{
			return this.Assemble(mgr);
		}

		public static byte GetAddressingMode(byte mod, byte reg, byte rm)
		{
			return (byte)((mod << 6) | (reg << 3) | rm); 
		}
	}
}
