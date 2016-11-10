using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler.Logic.Arguments;

namespace Assembler.Logic.Commands
{
	class Jxx: Command
	{
		public string CommandName { get; set; }
		public string LabelName { get; set; }
		public Lexer.CaptureInfo Capture { get; set; }

		public static Dictionary<string, byte> Codes { get; set; }

		static Jxx()
		{
			Codes = new Dictionary<string, byte>();
			Codes["JO"] = 0x00;
			Codes["JNO"] = 0x01;
			Codes["JB"] = Codes["JNAE"] = 0x02;
			Codes["JNB"] = Codes["JAE"] = 0x03;
			Codes["JE"] = Codes["JZ"] = 0x04;
			Codes["JNE"] = Codes["JNZ"] = 0x05;
			Codes["JBE"] = Codes["JNA"] = 0x06;
			Codes["JNBE"] = Codes["JA"] = 0x07;
			Codes["JS"] = 0x08;
			Codes["JNS"] = 0x09;
			Codes["JP"] = Codes["JPE"] = 0x0a;
			Codes["JNP"] = Codes["JPO"] = 0x0b;
			Codes["JL"] = Codes["JNGE"] = 0x0c;
			Codes["JNL"] = Codes["JGE"] = 0x0d;
			Codes["JLE"] = Codes["JNG"] = 0x0e;
			Codes["JNLE"] = Codes["JG"] = 0x0f;
		}

		byte[] assemble(MemoryManager mgr)
		{
			byte cmd8bit = 0x70;
			byte cmd16bitPrefix = 0x0f;
			byte cmd16bit = 0x80;
			if (!mgr.IsLabelDecalared(LabelName))
			{
				throw new Exceptions.LabelNotDeclaredException(LabelName, CommandName, Capture);
			}
			Int16 from = mgr.Pointer;
			Int16 to = mgr.Labels[LabelName];
			var shift = (Int16)(to - (from + 2));  // 8bit jump
			var cond = Codes[CommandName];
			if ((sbyte)shift == shift)
			{
				return new byte[] { (byte)(cmd8bit | cond), (byte)(sbyte)shift };
			}
			shift = (Int16)(to - (from + 4)); // 16 bit jump
			return new byte[] { cmd16bitPrefix, (byte)(cmd16bit | cond), (byte)shift, (byte)(shift >> 8) };
		}

		public static Jxx Create(ILineInfo line)
		{
			var command = line.Command.ToUpper();
			if (line.NumberOfArguments != 1)
			{
				throw new Exceptions.ArgumentNumberException(command, line.NumberOfArguments);
			}
			var cmd = new Jxx()
			{
				LineNumber = line.LineNumber,
				CommandName = command
			};
			switch (line.TypeOfArgument(1))
			{
				case Lexer.ArgumentType.Name:
					cmd.LabelName = line.Argument(1).ToLower();
					cmd.Capture = line.LastCapture;
					cmd.Assemble = cmd.assemble;
					break;
			}
			if (cmd.Assemble == null)
			{
				throw new Exceptions.ArgumentException(command, line.TypeOfArgument(1));
			}
			return cmd;
		}
	}
}
