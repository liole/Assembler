using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class ProcedureDoesNotReturn: LineException
	{
		public string ProcedureName { get; set; }

		public ProcedureDoesNotReturn(string procName, Lexer.CaptureInfo capture = null) :
			base(capture)
		{
			ProcedureName = procName;
		}

		public override string Message
		{
			get
			{
				return String.Format("Procedure '{0}' ended, but did not return control to the OS.", ProcedureName);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Procedure does not return error";
			}
		}
	}
}
