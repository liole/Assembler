using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	public class Compiler
	{
		public Program Program { get; set; }

		public byte[] Compile(string text)
		{
			var parser = new Parser(text);
			Program = parser.Parse();
			var preCode = Program.Assemble();	// pass 1 - calculate symbols addresses
			var code = Program.Assemble();		// pass 2 - get assembled code
			return code;
		}
	}
}
