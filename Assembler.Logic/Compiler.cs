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
		bool addExceptions = true;

		public byte[] Compile(string text, bool collectExceptions = true)
		{
			Exceptions = new List<Logic.Exceptions.LineException>();
			var parser = new Parser(text);
			attachHandler(parser, collectExceptions);

			Program = parser.Parse();
			attachHandler(collectExceptions);

			var pre1Code = Program.Assemble();	// pass 1 - calculate symbols addresses
			addExceptions = false;
			var pre2code = Program.Assemble();	// pass 2 - get 8/16 bit address shifts
			var code = Program.Assemble();		// pass 3 - get assembled code
			code = Program.Assemble();		// pass 4 - if addresses change for some reason
			
			dettachAllHandlers(parser, collectExceptions);
			return code;
		}

		void attachHandler(bool collectExceptions)
		{
			addExceptions = true;
			if (collectExceptions)
			{
				Program.ExceptionHandler += compiler_ExceptionHandler;
			}
		}

		void attachHandler(Parser parser, bool collectExceptions)
		{
			addExceptions = true;
			if (collectExceptions)
			{
				parser.ExceptionHandler += compiler_ExceptionHandler;
			}
		}

		void dettachAllHandlers(Parser parser, bool collectExceptions)
		{
			dettachHandler(parser, collectExceptions);
			dettachHandler(collectExceptions);
		}

		void dettachHandler(bool collectExceptions)
		{
			if (collectExceptions)
			{
				Program.ExceptionHandler -= compiler_ExceptionHandler;
			}
		}

		void dettachHandler(Parser parser, bool collectExceptions)
		{
			if (collectExceptions)
			{
				parser.ExceptionHandler -= compiler_ExceptionHandler;
			}
		}

		void compiler_ExceptionHandler(object sender, Exceptions.ExceptionHandlerArgs e)
		{
			if (addExceptions)
			{
				Exceptions.Add(e.Exception);
			}
		}
	}
}
