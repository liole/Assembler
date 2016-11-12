using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler.Logic.Arguments;

namespace Assembler.Logic.Commands
{
	class RET: Command
	{
		public Number Argument1 { get; set; }

		byte[] assemble(MemoryManager mgr)
		{
			var res = new List<byte>();
			if (Argument1 == null)
			{
				res.Add(0xc3);
			}
			else
			{
				res.Add(0xc2);
				res.AddRange(Argument1.GetData(true));
			}
			return res.ToArray();
		}

		public static RET Create(ILineInfo line)
		{
			if (line.NumberOfArguments > 1)
			{
				throw new Exceptions.ArgumentNumberException("RET", line.NumberOfArguments);
			}
			var cmd = new RET()
			{
				LineNumber = line.LineNumber
			};
			switch (line.TypeOfArgument(1))
			{
				case Lexer.ArgumentType.Number:
					cmd.Argument1 = new Number((Int16)line.ArgumentAsNumber(1));
					cmd.Assemble = cmd.assemble;
					break;
				case Lexer.ArgumentType.None:
					cmd.Assemble = cmd.assemble;
					break;
			}
			if (cmd.Assemble == null)
			{
				throw new Exceptions.ArgumentException("RET", line.TypeOfArgument(1));
			}
			return cmd;
		}
	}
}
