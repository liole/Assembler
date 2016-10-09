using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class CommandException: LineException
	{
		public string CommandName { get; set; }

		public CommandException(string commandName, Lexer.CaptureInfo capture = null) :
			base(capture)
		{
			CommandName = commandName;
		}

		public override string Message
		{
			get
			{
				return String.Format("There is a problem with command '{0}'", CommandName);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Command error";
			}
		}
	}
}
