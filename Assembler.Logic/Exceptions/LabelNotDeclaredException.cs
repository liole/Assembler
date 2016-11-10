using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class LabelNotDeclaredException: CommandException
	{
		public string LabelName { get; set; }

		public LabelNotDeclaredException(string labelName, string commandName, Lexer.CaptureInfo capture = null) :
			base(commandName, capture)
		{
			LabelName = labelName;
		}

		public override string Message
		{
			get
			{
				return String.Format("Label '{0}' is not declared, but used in command '{1}'.",
					LabelName, CommandName);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Label not declared error";
			}
		}
	}
}
