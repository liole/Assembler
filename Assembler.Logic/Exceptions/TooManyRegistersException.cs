using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class TooManyRegistersException: LineException
	{
		public int NumberOfRegisters { get; set; }

		public TooManyRegistersException(int numberOfRegisters, Lexer.CaptureInfo capture = null) :
			base(capture)
		{
			NumberOfRegisters = numberOfRegisters;
		}

		public override string Message
		{
			get
			{
				return String.Format("Can not use {0} registers for indirect addressing mode", NumberOfRegisters);
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Too many registers error";
			}
		}
	}
}
