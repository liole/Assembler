﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class ProcedureRedeclaredException: LineException
	{
		public string ProcedureName { get; set; }

		public ProcedureRedeclaredException(string procName, Lexer.CaptureInfo capture = null) :
			base(capture)
		{
			ProcedureName = procName;
		}

		public override string Message
		{
			get
			{
				return String.Format("Procedure '{0}' is already declared.", ProcedureName);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Procedure rederclared error";
			}
		}
	}
}
