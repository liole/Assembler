using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	class Parser
	{
		public string Text { get; set; }

		public Parser(string text)
		{
			Text = text;
		}

		public Program Parse()
		{
			var program = new Program();
			using (var lexer = new Lexer(Text))
			{
				while (lexer.NextLine())
				{
					if (lexer.HasLabel)
					{
						program.Add(new Label(lexer.Label, lexer.LineNumber));
					}
					switch (lexer.Type)
					{
						case Lexer.LineType.Command:
							var commandName = lexer.Command;
							var command = Commands.Creator.Create(commandName, lexer);
							program.Add(command);
							break;
						case Lexer.LineType.Definition:
							var definition = Definition.Create(lexer);
							program.Add(definition);
							break;
					}
				}
			}
			return program;
		}
	}
}
