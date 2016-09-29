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

		public CommandException(int lineNumber, string commandName):
			base(lineNumber)
		{
			CommandName = commandName;
		}

		public override string Message
		{
			get
			{
				return String.Format("There is a problem with command '{0}' in line {1}", CommandName, LineNumber);
			}
		}
	}
}
