using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class ProcedureAlreadyEndedException: LineException
	{
		public string ProcedureName { get; set; }

		public ProcedureAlreadyEndedException(string procName, Lexer.CaptureInfo capture = null) :
			base(capture)
		{
			ProcedureName = procName;
		}

		public override string Message
		{
			get
			{
				return String.Format("Procedure '{0}' is already ended.", ProcedureName);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Procedure already ended error";
			}
		}
	}
}
