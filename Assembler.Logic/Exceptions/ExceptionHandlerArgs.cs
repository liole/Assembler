using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class ExceptionHandlerArgs: EventArgs
	{
		public LineException Exception { get; set; }

		public ExceptionHandlerArgs(LineException exception)
		{
			Exception = exception;
		}
	}
}
