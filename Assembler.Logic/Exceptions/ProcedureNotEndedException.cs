using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class ProcedureNotEndedException: LineException
	{
		public string ProcedureName { get; set; }

		public ProcedureNotEndedException(string procName, Lexer.CaptureInfo capture = null) :
			base(capture)
		{
			ProcedureName = procName;
		}

		public override string Message
		{
			get
			{
				return String.Format("Procedure '{0}' is not ended.", ProcedureName);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Procedure not ended error";
			}
		}
	}
}
