using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class NotACommandException: CommandException
	{
		public NotACommandException(int lineNumber, string commandName) :
			base(lineNumber, commandName)
		{
			CommandName = commandName;
		}

		public override string Message
		{
			get
			{
				return String.Format("Command '{0}' is not recognized[Line {1}]", CommandName, LineNumber);
			}
		}
	}
}
