using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler.Logic.Arguments;

namespace Assembler.Logic.Commands
{
	class INT: Command
	{
		public Number Argument1 { get; set; }

		byte[] assemble(MemoryManager mgr)
		{
			byte cmd = 0xcd;
			var num = Argument1.GetByte();
			return new byte[] { cmd, num };
		}

		public static INT Create(ILineInfo line)
		{
			if (line.NumberOfArguments != 1)
			{
				throw new Exceptions.ArgumentNumberException("INT", line.NumberOfArguments);
			}
			var cmd = new INT()
			{
				LineNumber = line.LineNumber
			};
			switch (line.TypeOfArgument(1))
			{
				case Lexer.ArgumentType.Number:
					cmd.Argument1 = new Number((Int16)line.ArgumentAsNumber(1));
					if (cmd.Argument1.IsWord)
					{
						throw new Exceptions.ArgumentSizeException("INT");
					}
					cmd.Assemble = cmd.assemble;
					break;
			}
			if (cmd.Assemble == null)
			{
				throw new Exceptions.ArgumentException("INT", line.TypeOfArgument(1));
			}
			return cmd;
		}
	}
}
