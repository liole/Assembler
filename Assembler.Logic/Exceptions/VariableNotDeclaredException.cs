using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class VariableNotDeclaredException: CommandException
	{
		public string VariableName { get; set; }

		public VariableNotDeclaredException(string varName, int lineNumber, string commandName) :
			base(lineNumber, commandName)
		{
			VariableName = varName;
		}

		public override string Message
		{
			get
			{
				return String.Format("Variable '{0}' is not declared, but used in command '{1}'.[Line {2}]",
					VariableName, CommandName, LineNumber);
			}
		}
	}
}
