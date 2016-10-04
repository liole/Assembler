using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Arguments
{
	public class Register: IArgument
	{
		public string Type { get; set; }

		public Register(string type)
		{
			Type = type.ToUpper();
		}

		public byte Code
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

		public static Dictionary<string, byte> Codes { get; set; }

		static Register()
		{
			Codes = new Dictionary<string, byte>();
			Codes["AX"] = Codes["AL"] = 0x00;
			Codes["CX"] = Codes["CL"] = 0x01;
			Codes["DX"] = Codes["DL"] = 0x02;
			Codes["BX"] = Codes["BL"] = 0x03;

			Codes["SP"] = Codes["AH"] = 0x04;
			Codes["BP"] = Codes["CH"] = 0x05;
			Codes["SI"] = Codes["DH"] = 0x06;
			Codes["DI"] = Codes["BH"] = 0x07;
		}

	}
}
