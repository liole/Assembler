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

		public ArgumentNumberException(int lineNumber, string commandName, int numberOfArguments):
			base(lineNumber, commandName)
		{
			NumberOfArguments = numberOfArguments;
		}

		public override string Message
		{
			get
			{
				return String.Format("Command '{0}' can not have {1} argument(s).[Line {3}]",
					CommandName, NumberOfArguments, LineNumber);
			}
		}
	}
}
