using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Arguments
{
	public class Register: Argument
	{
		public string Type { get; set; }

		public Register(string type)
		{
			Type = type;
		}

		public Int16 Code
		{
			get
			{
				return Codes[Type];
			}
		}

		public bool IsWord
		{
			get
			{
				return Type[1] != 'L' && Type[1] != 'H';
			}
		}

		public static Dictionary<string, Int16> Codes { get; set; }

		static Register()
		{
			Codes["AX"] = Codes["AL"] = 0x00;
			Codes["BX"] = Codes["BL"] = 0x01;
			Codes["CX"] = Codes["CL"] = 0x02;
			Codes["DX"] = Codes["DL"] = 0x03;

			Codes["SP"] = Codes["AH"] = 0x04;
			Codes["BP"] = Codes["BH"] = 0x05;
			Codes["SI"] = Codes["CH"] = 0x06;
			Codes["DI"] = Codes["DH"] = 0x07;
		}

	}
}
