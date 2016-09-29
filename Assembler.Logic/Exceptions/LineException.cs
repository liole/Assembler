using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class LineException: Exception
	{
		public int LineNumber { get; set; }

		public LineException(int lineNumber)
		{
			LineNumber = lineNumber;
		}

		public override string Message
		{
			get
			{
				return String.Format("There is a problem in line {0}", LineNumber);
			}
		}
	}
}
