using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	class Parser: IDisposable
	{
		private Lexer lexer;

		public Parser(string text)
		{
			lexer = new Lexer(text);
		}

		public Program Parse()
		{
			var program = new Program();
			while (lexer.NextLine())
			{
				if (lexer.HasLabel)
				{
					program.Add(new Label(lexer.Label));
				}
				switch (lexer.Type)
				{
					case Lexer.LineType.Command:
						var commandName = lexer.Command;
						var command = Commands.Creator.Create(commandName, lexer);
						program.Add(command);
						break;
					case Lexer.LineType.Definition:

						break;
				}
			}
			return program;
		}

		public void Dispose()
		{
			lexer.Dispose();
		}
	}
}
