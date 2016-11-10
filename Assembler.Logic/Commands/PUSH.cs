using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler.Logic.Arguments;

namespace Assembler.Logic.Commands
{
	class PUSH: Command
	{
		public IArgument Argument1 { get; set; }

		public bool CheckArgumentSize()
		{
			if (Argument1.IsWord || Argument1 is Number)
			{
				return Argument1.IsWord; 
			}
			throw new Exceptions.ArgumentSizeException("PUSH");
		}

		byte[] assembleR(MemoryManager mgr)
		{
			byte cmd = 0x50;
			var reg = (Argument1 as Register).Code;
			var cmdreg = (byte)(cmd | reg);
			return new byte[] { cmdreg };
		}

		byte[] assembleM(MemoryManager mgr)
		{
			byte cmd = 0xff;
			var mem = Argument1 as Memory;
			var declared = mem.Attach(mgr);
			if (!declared)
			{
				throw new Exceptions.VariableNotDeclaredException(mem.Name, "PUSH", mem.Capture);
			}
			CheckArgumentSize();
			var addr = mem.GetData();
			byte mod = mem.MOD;
			byte reg = 0x06;
			byte rm = mem.RM;
			var modrm = Command.GetAddressingMode(mod, reg, rm);
			mem.Detach();
			var res = new List<byte>() { cmd, modrm };
			res.AddRange(addr);
			return res.ToArray();
		}

		byte[] assembleI(MemoryManager mgr)
		{
			byte cmd = 0x68;
			var im = (Argument1 as Number);
			var w = CheckArgumentSize();
			var s = (byte)(w ? 0 : (1 << 1));
			var cmds = (byte)(cmd | s);
			var res = new List<byte>() { cmds };
			res.AddRange(im.GetValue(w));
			return res.ToArray();
		}

		public static PUSH Create(ILineInfo line)
		{
			if (line.NumberOfArguments != 1)
			{
				throw new Exceptions.ArgumentNumberException("PUSH", line.NumberOfArguments);
			}
			var cmd = new PUSH()
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
				case Lexer.ArgumentType.Number:
					cmd.Argument1 = new Number((Int16)line.ArgumentAsNumber(1));
					cmd.Assemble = cmd.assembleI;
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
				throw new Exceptions.ArgumentException("PUSH", line.TypeOfArgument(1));
			}
			if (!isMem)
			{
				cmd.CheckArgumentSize();
			}
			return cmd;
		}
	}
}
