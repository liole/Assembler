using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class ProcedureNotDeclaredException: CommandException
	{
		public string ProcedureName { get; set; }

		public ProcedureNotDeclaredException(string procName, string commandName, Lexer.CaptureInfo capture = null) :
			base(commandName, capture)
		{
			ProcedureName = procName;
		}

		public override string Message
		{
			get
			{
				return String.Format("Procedure '{0}' is not declared, but used in command '{1}'.",
					ProcedureName, CommandName);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Procedure not declared error";
			}
		}
	}
}
