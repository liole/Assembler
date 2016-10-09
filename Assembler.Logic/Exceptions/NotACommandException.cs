using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class NotACommandException: CommandException
	{
		public NotACommandException(string commandName, Lexer.CaptureInfo capture = null) :
			base(commandName, capture)
		{
		}

		public override string Message
		{
			get
			{
				return String.Format("Command '{0}' is not recognized.", CommandName);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Not a command error";
			}
		}
	}
}
