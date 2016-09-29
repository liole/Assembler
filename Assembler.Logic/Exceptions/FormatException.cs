using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class FormatException: LineException
	{
		public FormatException(int lineNumber) :
			base(lineNumber)
		{
		}

		public override string Message
		{
			get
			{
				return String.Format("Line {0} has incorrect format", LineNumber);
			}
		}
	}
}
