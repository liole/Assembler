using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class FormatException: LineException
	{
		public FormatException(Lexer.CaptureInfo capture = null) :
			base(capture)
		{
		}

		public override string Message
		{
			get
			{
				return String.Format("Line {0} has incorrect format", LineNumber + 1);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Format error";
			}
		}
	}
}
