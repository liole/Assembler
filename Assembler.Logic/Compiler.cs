using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	public class Compiler
	{
		public Program Program { get; private set; }
		public List<Exceptions.LineException> Exceptions { get; private set; }

		public byte[] Compile(string text, bool collectExceptions = true)
		{
			Exceptions = new List<Logic.Exceptions.LineException>();
			var parser = new Parser(text);
			if (collectExceptions)
			{
				parser.ExceptionHandler += compiler_ExceptionHandler;
			}
			Program = parser.Parse();
			if (collectExceptions)
			{
				Program.ExceptionHandler += compiler_ExceptionHandler;
			}
			var pre1Code = Program.Assemble();	// pass 1 - calculate symbols addresses
			var pre2code = Program.Assemble();	// pass 2 - get 8/16 bit address shifts
			var code = Program.Assemble();		// pass 3 - get assembled code
			return code;
		}

		void compiler_ExceptionHandler(object sender, Exceptions.ExceptionHandlerArgs e)
		{
			Exceptions.Add(e.Exception);
		}
	}
}
