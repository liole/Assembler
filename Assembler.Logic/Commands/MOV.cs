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
		
		public Func<MemoryManager, byte[]> FactoryAssemble(byte cmd)
		{
			return Factory.Assemble2Args(cmd, "MOV", this.Argument1, this.Argument2);
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
							cmd.Assemble = cmd.FactoryAssemble(0x88); //cmd.assembleRR;
							break;
						case Lexer.ArgumentType.Number:
							cmd.Argument2 = new Number((Int16)line.ArgumentAsNumber(2));
							cmd.Assemble =  cmd.assembleRI;
							break;
						case Lexer.ArgumentType.Name:
						case Lexer.ArgumentType.Indirect:
							cmd.Argument2 = new MemoryIndirect(
								line.NameInArgument(2),
								line.RegistersInArgument(2),
								line.NumbersInArgument(2),
								line.LastCapture
							);
							cmd.Assemble =  cmd.FactoryAssemble(0x8a); //cmd.assembleRM;
							isMem = true;
							break;
					}
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
					switch (line.TypeOfArgument(2))	
					{
						case Lexer.ArgumentType.Register:
							cmd.Argument2 = new Register(line.Argument(2));
							cmd.Assemble = cmd.FactoryAssemble(0x88); //cmd.assembleMR;
							break;
						case Lexer.ArgumentType.Number:
							cmd.Argument2 = new Number((Int16)line.ArgumentAsNumber(2));
							cmd.Assemble = cmd.FactoryAssemble(0xc6); //cmd.assembleMI;
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
