using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class ArgumentNumberException: CommandException
	{
		public int NumberOfArguments { get; set; }

		public ArgumentNumberException(string commandName, int numberOfArguments, Lexer.CaptureInfo capture = null) :
			base(commandName, capture)
		{
			NumberOfArguments = numberOfArguments;
		}

		public override string Message
		{
			get
			{
				return String.Format("Command '{0}' can not have {1} argument(s).",
					CommandName, NumberOfArguments);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Argument number error";
			}
		}
	}
}
