using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler.Logic.Arguments;

namespace Assembler.Logic.Commands
{
	class POP: Command
	{
		public IArgument Argument1 { get; set; }

		public bool CheckArgumentSize()
		{
			if (!Argument1.IsWord)
			{
				throw new Exceptions.ArgumentSizeException("POP");
			}
			return true;
		}

		byte[] assembleR(MemoryManager mgr)
		{
			byte cmd = 0x58;
			var reg = (Argument1 as Register).Code;
			var cmdreg = (byte)(cmd | reg);
			return new byte[] { cmdreg };
		}

		byte[] assembleM(MemoryManager mgr)
		{
			byte cmd = 0x8f;
			var mem = Argument1 as Memory;
			var declared = mem.Attach(mgr);
			if (!declared)
			{
				throw new Exceptions.VariableNotDeclaredException(mem.Name, "POP", mem.Capture);
			}
			CheckArgumentSize();
			var addr = mem.GetData();
			byte mod = mem.MOD;
			byte reg = 0x00;
			byte rm = mem.RM;
			var modrm = Command.GetAddressingMode(mod, reg, rm);
			mem.Detach();
			var res = new List<byte>() { cmd, modrm };
			res.AddRange(addr);
			return res.ToArray();
		}

		public static POP Create(ILineInfo line)
		{
			if (line.NumberOfArguments != 1)
			{
				throw new Exceptions.ArgumentNumberException("POP", line.NumberOfArguments);
			}
			var cmd = new POP()
			{
				LineNumber = line.LineNumber
			};
			bool isMem = false;
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
					isMem = true;
					cmd.Assemble = cmd.assembleM;
					break;
			}
			if (cmd.Assemble == null)
			{
				throw new Exceptions.ArgumentException("POP", line.TypeOfArgument(1));
			}
			if (!isMem)
			{
				cmd.CheckArgumentSize();
			}
			return cmd;
		}
	}
}
