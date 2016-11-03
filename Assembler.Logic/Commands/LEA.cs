using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler.Logic.Arguments;

namespace Assembler.Logic.Commands
{
	class LEA: Command
	{
		public Register Argument1 { get; set; }
		public IArgument Argument2 { get; set; }

		byte[] assembleRM(MemoryManager mgr)
		{
			byte cmd = 0x8d;
			byte mod = 0x00;
			var reg = Argument1.Code;
			byte rm = 0x06;
			var modrm = Command.GetAddressingMode(mod, reg, rm);
			var mem = Argument2 as MemoryName;
			var declared = mem.Attach(mgr);
			if (!declared)
			{
				throw new Exceptions.VariableNotDeclaredException(mem.Name, "LEA", mem.Capture);
			}
			var addr = mem.GetReverseAddress();
			mem.Detach();
			var res = new List<byte>() { cmd, modrm };
			res.AddRange(addr);
			return res.ToArray();
		}
		
		public static LEA Create(ILineInfo line)
		{
			if (line.NumberOfArguments != 2)
			{
				throw new Exceptions.ArgumentNumberException("LEA", line.NumberOfArguments);
			}
			var cmd = new LEA()
			{
				LineNumber = line.LineNumber
			};
			switch (line.TypeOfArgument(1))
			{
				case Lexer.ArgumentType.Register:
					cmd.Argument1 = new Register(line.Argument(1));
					if (!cmd.Argument1.IsWord)
					{
						// TODO: make better exception
						throw new Exceptions.ArgumentSizeException("LEA");
					}
					switch (line.TypeOfArgument(2))	
					{
						case Lexer.ArgumentType.Name:
							cmd.Argument2 = new MemoryName(line.Argument(2), line.LastCapture);
							cmd.Assemble =  cmd.assembleRM;
							break;
					}
					break;
			}
			if (cmd.Assemble == null)
			{
				// need better exception ?
				throw new Exceptions.ArgumentException("LEA", line.TypeOfArgument(1), line.TypeOfArgument(2));
			}
			return cmd;
		}
	}
}
