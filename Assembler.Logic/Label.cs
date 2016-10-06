using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	class Label: IInstruction
	{
		public int LineNumber { get; set; }
		public string Name { get; set; }

		public Label(string name, int lineNumber)
		{
			Name = name;
			LineNumber = lineNumber;
		}

		public byte[] Assemble(MemoryManager mgr)
		{
			throw new NotImplementedException();
		}
	}
}
