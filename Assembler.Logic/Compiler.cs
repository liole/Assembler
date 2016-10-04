using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	public class Compiler
	{
		public byte[] Compile(string text)
		{
			var parser = new Parser(text);
			var program = parser.Parse();
			var preCode = program.Assemble();	// pass 1 - calculate symbols addresses
			var code = program.Assemble();		// pass 2 - get assembled code
			return code;
		}
	}
}
