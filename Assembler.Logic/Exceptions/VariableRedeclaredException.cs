using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class VariableRedeclaredException: LineException
	{
		public string VariableName { get; set; }

		public VariableRedeclaredException(string varName, Lexer.CaptureInfo capture = null) :
			base(capture)
		{
			VariableName = varName;
		}

		public override string Message
		{
			get
			{
				return String.Format("Variable '{0}' is already declared.", VariableName);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Variable rederclared error";
			}
		}
	}
}
