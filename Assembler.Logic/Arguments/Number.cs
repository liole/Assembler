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
				//return Value >> 8 != 0;
				var b = (byte)Value;
				return (byte)Value != Value && (sbyte)Value != Value;
			}
		}

		public byte GetByte()
		{
			return (byte)Value;
		}

		public byte[] GetReverseWord()
		{
			return new[] { (byte)Value, (byte)(Value >> 8) };
		}

		public byte[] GetValue(bool word)
		{
			return word ? GetReverseWord() : new[] { GetByte() };
		}
	}
}
