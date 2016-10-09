using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler.Logic.Arguments;

namespace Assembler.Logic.Commands
{
	class ORG: Command
	{
		public Number Argument1 { get; set; }

		byte[] assemble(MemoryManager mgr)
		{
			var pointer = Argument1.Value;
			mgr.ResetPointer(pointer);
			return new byte[] { };
		}

		public static ORG Create(ILineInfo line)
		{
			if (line.NumberOfArguments != 1)
			{
				throw new Exceptions.ArgumentNumberException("ORG", line.NumberOfArguments);
			}
			var cmd = new ORG()
			{
				LineNumber = line.LineNumber
			};
			switch (line.TypeOfArgument(1))
			{
				case Lexer.ArgumentType.Number:
					cmd.Argument1 = new Number((Int16)line.ArgumentAsNumber(1));
					cmd.Assemble = cmd.assemble;
					break;
			}
			if (cmd.Assemble == null)
			{
				throw new Exceptions.ArgumentException("ORG", line.TypeOfArgument(1));
			}
			return cmd;
		}
	}
}
