using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler.Logic.Arguments;

namespace Assembler.Logic.Commands
{
	class MOV: Command
	{
		public IArgument Argument1 { get; set; }
		public IArgument Argument2 { get; set; }

		public bool CheckArgumentSize()
		{
			if (Argument1.IsWord == Argument2.IsWord)
			{
				return Argument1.IsWord;
			}
			if (Argument2 is Number && Argument1.IsWord)
			{
				return true;
			}
			throw new Exceptions.ArgumentSizeException(0, "MOV"); // get line number from somewhere ...
		}

		public byte[] AssembleRR(MemoryManager mgr)
		{
			byte cmd = 0x88;
			var w = CheckArgumentSize();
			var cmdw = (byte)(cmd | (w ? 1 : 0));
			byte mod = 0x03;
			var reg = (Argument2 as Register).Code;
			var rm = (Argument1 as Register).Code;
			var modrm = Command.GetAddressingMode(mod, reg, rm);
			return new[] { cmdw, modrm };
		}

		public byte[] AssembleRI(MemoryManager mgr)
		{
			byte cmd = 0xb0;
			var w = CheckArgumentSize();
			var cmdw = (byte)(cmd | ((w ? 1 : 0) << 3));
			var reg = (Argument1 as Register).Code;
			var cmdwreg = (byte)(cmdw | reg);
			var im = (Argument2 as Number).GetValue(w);
			var res = new List<byte>() { cmdwreg };
			res.AddRange(im);
			return res.ToArray();
		}
		
		public static MOV Create(ILineInfo line)
		{
			if (line.NumberOfArguments != 2)
			{
				throw new Exceptions.ArgumentNumberException(line.LineNumber, "MOV", line.NumberOfArguments);
			}
			var cmd = new MOV();
			switch (line.TypeOfArgument(1))
			{
				case Lexer.ArgumentType.Register:
					cmd.Argument1 = new Register(line.Argument(1));
					switch (line.TypeOfArgument(2))	
					{
						case Lexer.ArgumentType.Register:
							cmd.Argument2 = new Register(line.Argument(2));
							cmd.Assemble = cmd.AssembleRR;
							break;
						case Lexer.ArgumentType.Number:
							cmd.Argument2 = new Number((Int16)line.ArgumentAsNumber(2));
							cmd.Assemble = cmd.AssembleRI;
							break;
						case Lexer.ArgumentType.Name:
							break;
					}
					break;
				case Lexer.ArgumentType.Name:
					break;
			}
			if (cmd.Assemble == null)
			{
				throw new Exceptions.ArgumentException(
					line.LineNumber, "MOV", line.TypeOfArgument(1), line.TypeOfArgument(2));
			}
			return cmd;
		}
	}
}
