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

		public event EventHandler<Exceptions.ExceptionHandlerArgs> ExceptionHandler;
		private bool formatException = false;

		public Parser(string text)
		{
			Text = text;
		}

		public Program Parse()
		{
			var program = new Program();
			using (var lexer = new Lexer(Text))
			{
				lexer.ExceptionHandler += parser_ExceptionHandler;
				while (lexer.NextLine())
				{
					if (formatException)
					{
						formatException = false;
						continue;
					}
					try
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
					catch(Exceptions.LineException e)
					{
						if (e.CaptureInfo == null)
						{
							e.CaptureInfo = lexer.LastCapture;
						}
						parser_ExceptionHandler(this, new Exceptions.ExceptionHandlerArgs(e));
					}
				}
			}
			return program;
		}

		private void parser_ExceptionHandler(object sender, Exceptions.ExceptionHandlerArgs e)
		{
			if (ExceptionHandler != null)
			{
				ExceptionHandler(sender, e);
				if (e.Exception is Exceptions.FormatException)
				{
					formatException = true;
				}
			}
			else
			{
				throw e.Exception;
			}
		}
	}
}
