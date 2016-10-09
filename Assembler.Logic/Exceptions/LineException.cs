using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class LineException: Exception
	{
		public Lexer.CaptureInfo CaptureInfo { get; set; }
		public int LineNumber
		{
			get
			{
				return CaptureInfo.LineNumber;
			}
		}

		public LineException(Lexer.CaptureInfo capture = null)
		{
			CaptureInfo = capture;
		}

		public override string Message
		{
			get
			{
				return String.Format("There is a problem in line {0}", LineNumber + 1);
			}
		}

		public virtual string ErrorName
		{
			get
			{
				return "Line error";
			}
		}
	}
}
