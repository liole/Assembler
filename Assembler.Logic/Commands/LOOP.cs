using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler.Logic.Arguments;

namespace Assembler.Logic.Commands
{
	class LOOP: Command
	{
		public string LabelName { get; set; }
		public Lexer.CaptureInfo Capture { get; set; }

		byte[] assemble(MemoryManager mgr)
		{
			byte cmd = 0xe2;
			if (!mgr.IsLabelDecalared(LabelName))
			{
				throw new Exceptions.LabelNotDeclaredException(LabelName, "LOOP", Capture);
			}
			Int16 from = mgr.Pointer;
			Int16 to = mgr.Labels[LabelName];
			var shift = (Int16)(to - (from + 2));  // 8bit jump only
			if ((sbyte)shift == shift || to == MemoryManager.UndefinedAddress)
			{
				return new byte[] { cmd, (byte)(sbyte)shift };
			}
			throw new Exceptions.LoopTooFarException(LabelName, Capture);
		}

		public static LOOP Create(ILineInfo line)
		{
			if (line.NumberOfArguments != 1)
			{
				throw new Exceptions.ArgumentNumberException("LOOP", line.NumberOfArguments);
			}
			var cmd = new LOOP()
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
				throw new Exceptions.ArgumentException("LOOP", line.TypeOfArgument(1));
			}
			return cmd;
		}
	}
}
