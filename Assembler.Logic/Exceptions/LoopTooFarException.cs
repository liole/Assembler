using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class LoopTooFarException: CommandException
	{
		public string LabelName { get; set; }

		public LoopTooFarException(string labelName, Lexer.CaptureInfo capture = null) :
			base("LOOP", capture)
		{
			LabelName = labelName;
		}

		public override string Message
		{
			get
			{
				return String.Format("Command '{1}' only supports short jumps (+/-128 bytes). Label '{0}' is too far.",
					LabelName, CommandName);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Loop too far error";
			}
		}
	}
}
