using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler.Logic.Arguments;

namespace Assembler.Logic.Commands
{
	class DEC: Command
	{
		public IArgument Argument1 { get; set; }

		byte[] assembleR(MemoryManager mgr)
		{
			byte cmd = 0x48;
			var reg = (Argument1 as Register).Code;
			var cmdreg = (byte)(cmd | reg);
			return new byte[] { cmdreg };
		}

		public Func<MemoryManager, byte[]> FactoryAssemble(byte cmd)
		{
			return Factory.Assemble1Arg(cmd, "DEC", this.Argument1, 0x01);
		}

		public static DEC Create(ILineInfo line)
		{
			if (line.NumberOfArguments != 1)
			{
				throw new Exceptions.ArgumentNumberException("DEC", line.NumberOfArguments);
			}
			var cmd = new DEC()
			{
				LineNumber = line.LineNumber
			};
			switch (line.TypeOfArgument(1))
			{
				case Lexer.ArgumentType.Register:
					cmd.Argument1 = new Register(line.Argument(1));
					cmd.Assemble = cmd.assembleR;
					break;
				case Lexer.ArgumentType.Name:
				case Lexer.ArgumentType.Indirect:
					cmd.Argument1 = new MemoryIndirect(
						line.NameInArgument(1),
						line.RegistersInArgument(1),
						line.NumbersInArgument(1),
						line.LastCapture
					);
					cmd.Assemble = cmd.FactoryAssemble(0xfe);
					break;
			}
			if (cmd.Assemble == null)
			{
				throw new Exceptions.ArgumentException("DEC", line.TypeOfArgument(1));
			}
			return cmd;
		}
	}
}
