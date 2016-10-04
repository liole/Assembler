using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Arguments
{
	class Number: IArgument
	{
		public Int16 Value { get; set; }

		public Number(Int16 value)
		{
			Value = value;
		}

		public bool IsWord
		{
			get
			{
				return Value >> 8 != 0;
			}
		}
	}
}
