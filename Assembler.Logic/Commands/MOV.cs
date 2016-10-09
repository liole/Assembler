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
			throw new Exceptions.ArgumentSizeException("MOV");
		}

		byte[] assembleRR(MemoryManager mgr)
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

		byte[] assembleRI(MemoryManager mgr)
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

		byte[] assembleRMR(MemoryManager mgr, byte direction)
		{
			byte cmd = 0x88;
			Register regObj;
			MemoryName mem;
			if (direction == 0)
			{
				regObj = Argument2 as Register;
				mem = Argument1 as MemoryName;
			}
			else
			{
				regObj = Argument1 as Register;
				mem = Argument2 as MemoryName;
			}
			byte reg = regObj.Code;
			//var regA = false;
			//if (regObj.Type[0] == 'A')
			//{
			//	cmd = 0xa0;
			//	direction = (byte)(direction == 0 ? 1 : 0);
			//	regA = true;
			//}
			var declared = mem.Attach(mgr);
			if (!declared)
			{
				throw new Exceptions.VariableNotDeclaredException(mem.Name, "MOV", mem.Capture);
			}
			var addr = mem.GetReverseAddress();
			var w = CheckArgumentSize();
			var cmddw = (byte)(cmd | (w ? 1 : 0) | (direction << 1));
			mem.Detach();
			var res = new List<byte>() { cmddw };
			//if (!regA)
			//{
			//	byte mod = 0x00;
			//	byte rm = 0x06;
			//	var modrm = Command.GetAddressingMode(mod, reg, rm);
			//	res.Add(modrm);
			//}
			res.AddRange(addr);
			return res.ToArray();
		}

		byte[] assembleRM(MemoryManager mgr)
		{
			return assembleRMR(mgr, 1);
		}

		byte[] assembleMR(MemoryManager mgr)
		{
			return assembleRMR(mgr, 0);
		}

		byte[] assembleMI(MemoryManager mgr)
		{
			byte cmd = 0xc6;
			var mem = Argument1 as MemoryName;
			var declared = mem.Attach(mgr);
			if (!declared)
			{
				throw new Exceptions.VariableNotDeclaredException(mem.Name, "MOV", mem.Capture);
			}
			var addr = mem.GetReverseAddress();
			var w = CheckArgumentSize();
			mem.Detach();
			var cmdw = (byte)(cmd | (w ? 1 : 0));
			byte reg = 0x00;
			byte mod = 0x00;
			byte rm = 0x06;
			var modrm = Command.GetAddressingMode(mod, reg, rm);
			var im = (Argument2 as Number).GetValue(w);
			var res = new List<byte>() { cmdw, modrm };
			res.AddRange(addr);
			res.AddRange(im);
			return res.ToArray();
		}
		
		public static MOV Create(ILineInfo line)
		{
			if (line.NumberOfArguments != 2)
			{
				throw new Exceptions.ArgumentNumberException("MOV", line.NumberOfArguments);
			}
			var cmd = new MOV()
			{
				LineNumber = line.LineNumber
			};
			bool isMem = false;
			switch (line.TypeOfArgument(1))
			{
				case Lexer.ArgumentType.Register:
					cmd.Argument1 = new Register(line.Argument(1));
					switch (line.TypeOfArgument(2))	
					{
						case Lexer.ArgumentType.Register:
							cmd.Argument2 = new Register(line.Argument(2));
							cmd.Assemble = cmd.assembleRR;
							break;
						case Lexer.ArgumentType.Number:
							cmd.Argument2 = new Number((Int16)line.ArgumentAsNumber(2));
							cmd.Assemble = cmd.assembleRI;
							break;
						case Lexer.ArgumentType.Name:
							cmd.Argument2 = new MemoryName(line.Argument(2), line.LastCapture);
							cmd.Assemble = cmd.assembleRM;
							isMem = true;
							break;
					}
					break;
				case Lexer.ArgumentType.Name:
					cmd.Argument1 = new MemoryName(line.Argument(1), line.LastCapture);
					isMem = true;
					switch (line.TypeOfArgument(2))	
					{
						case Lexer.ArgumentType.Register:
							cmd.Argument2 = new Register(line.Argument(2));
							cmd.Assemble = cmd.assembleMR;
							break;
						case Lexer.ArgumentType.Number:
							cmd.Argument2 = new Number((Int16)line.ArgumentAsNumber(2));
							cmd.Assemble = cmd.assembleMI;
							break;
					}
					break;
			}
			if (cmd.Assemble == null)
			{
				var lastCapture = line.LastCapture;
				throw new Exceptions.ArgumentException("MOV", line.TypeOfArgument(1), line.TypeOfArgument(2), lastCapture);
			}
			if (!isMem)
			{
				cmd.CheckArgumentSize();
			}
			return cmd;
		}
	}
}
