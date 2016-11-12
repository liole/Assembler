using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler.Logic.Arguments;

namespace Assembler.Logic.Commands
{
	class JMP: Command
	{
		public string LabelName { get; set; }
		public Lexer.CaptureInfo Capture { get; set; }

		byte[] assemble(MemoryManager mgr)
		{
			if (!mgr.IsLabelDecalared(LabelName))
			{
				throw new Exceptions.LabelNotDeclaredException(LabelName, "JMP", Capture);
			}
			Int16 from = mgr.Pointer;
			Int16 to = mgr.Labels[LabelName];
			return JMP.Code(from, to);
		}

		public static byte[] Code(Int16 from, Int16 to)
		{
			byte cmd8bit = 0xeb;
			byte cmd16bit = 0xe9;
			var shift = (Int16)(to - (from + 2));  // 8bit jump
			if ((sbyte)shift == shift)
			{
				return new byte[] { cmd8bit, (byte)(sbyte)shift };
			}
			shift = (Int16)(to - (from + 3)); // 16 bit jump
			return new byte[] { cmd16bit, (byte)shift, (byte)(shift >> 8) };
		}

		public static JMP Create(ILineInfo line)
		{
			if (line.NumberOfArguments != 1)
			{
				throw new Exceptions.ArgumentNumberException("JMP", line.NumberOfArguments);
			}
			var cmd = new JMP()
			{
				LineNumber = line.LineNumber
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
				throw new Exceptions.ArgumentException("JMP", line.TypeOfArgument(1));
			}
			return cmd;
		}
	}
}
