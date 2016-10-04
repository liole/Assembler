using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	class ArgumentSizeException: CommandException
	{
		public int NumberOfArguments { get; set; }

		public ArgumentSizeException(int lineNumber, string commandName):
			base(lineNumber, commandName)
		{
		}

		public override string Message
		{
			get
			{
				return String.Format("Sizes of arguments for command '{0}' are incompatible.[Line {1}]",
					CommandName, LineNumber);
			}
		}
	}
}
