using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class LabelRedeclaredException: LineException
	{
		public string LabelName { get; set; }

		public LabelRedeclaredException(string labelName, Lexer.CaptureInfo capture = null) :
			base(capture)
		{
			LabelName = labelName;
		}

		public override string Message
		{
			get
			{
				return String.Format("Label '{0}' is already declared.", LabelName);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Label rederclared error";
			}
		}
	}
}
