using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler.Logic.Arguments;

namespace Assembler.Logic.Commands
{
	class MUL: Command
	{
		public IArgument Argument1 { get; set; }

		public Func<MemoryManager, byte[]> FactoryAssemble(byte cmd)
		{
			return Factory.Assemble1Arg(cmd, "MUL", this.Argument1, 0x04);
		}

		public static MUL Create(ILineInfo line)
		{
			if (line.NumberOfArguments != 1)
			{
				throw new Exceptions.ArgumentNumberException("MUL", line.NumberOfArguments);
			}
			var cmd = new MUL()
			{
				LineNumber = line.LineNumber
			};
			switch (line.TypeOfArgument(1))
			{
				case Lexer.ArgumentType.Register:
					cmd.Argument1 = new Register(line.Argument(1));
					cmd.Assemble = cmd.FactoryAssemble(0xf6);
					break;
				case Lexer.ArgumentType.Name:
				case Lexer.ArgumentType.Indirect:
					cmd.Argument1 = new MemoryIndirect(
						line.NameInArgument(1),
						line.RegistersInArgument(1),
						line.NumbersInArgument(1),
						line.LastCapture
					);
					cmd.Assemble = cmd.FactoryAssemble(0xf6);
					break;
			}
			if (cmd.Assemble == null)
			{
				throw new Exceptions.ArgumentException("MUL", line.TypeOfArgument(1));
			}
			return cmd;
		}
	}
}
