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

		public VariableRedeclaredException(int lineNumber, string varName) :
			base(lineNumber)
		{
			VariableName = varName;
		}

		public override string Message
		{
			get
			{
				return String.Format("Variable '{0}' is already declared.[Line {1}]", VariableName, LineNumber);
			}
		}
	}
}
