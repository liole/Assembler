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

		public OverflowException(int lineNumber, string value):
			base(lineNumber)
		{
			Value = value;
		}

		public override string Message
		{
			get
			{
				return String.Format("Value {0} is too large for 16bit mode.[Line {1}]", Value, LineNumber);
			}
		}
	}
}
