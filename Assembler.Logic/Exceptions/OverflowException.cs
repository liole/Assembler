using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class OverflowException: LineException
	{
		public string Value { get; set; }

		public OverflowException(string value, Lexer.CaptureInfo capture = null) :
			base(capture)
		{
			Value = value;
		}

		public override string Message
		{
			get
			{
				return String.Format("Value {0} is too large for 16bit mode.", Value);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Overflow error";
			}
		}
	}
}
