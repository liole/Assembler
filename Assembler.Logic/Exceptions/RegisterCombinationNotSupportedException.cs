using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class RegisterCombinationNotSupportedException: LineException
	{
		public string RegisterCombination { get; set; }

		public RegisterCombinationNotSupportedException(string regCombination, Lexer.CaptureInfo capture = null) :
			base(capture)
		{
			RegisterCombination = regCombination;
		}

		public override string Message
		{
			get
			{
				return String.Format("Can not use register combination '{0}' for indirect addressing mode.", RegisterCombination);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Register combination not supported";
			}
		}
	}
}
