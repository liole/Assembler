using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class ArgumentSizeException: CommandException
	{
		public ArgumentSizeException(string commandName, Lexer.CaptureInfo capture = null) :
			base(commandName, capture)
		{
		}

		public override string Message
		{
			get
			{
				return String.Format("Size of argument(s) for command '{0}' is incompatible.",
					CommandName);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Argument size error";
			}
		}
	}
}
