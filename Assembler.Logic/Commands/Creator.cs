using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Commands
{
	class Creator
	{
		private static Dictionary<string, Func<ILineInfo, Command>> commands;

		static Creator()
		{
			commands = new Dictionary<string, Func<ILineInfo, Command>>();
			commands["org"] = ORG.Create;
			commands["int"] = INT.Create;

			commands["mov"] = MOV.Create;
			commands["lea"] = LEA.Create;

			commands["push"] = PUSH.Create;
			commands["pop"] = POP.Create;

			commands["add"] = ADD.Create;
			commands["sub"] = SUB.Create;
			commands["mul"] = MUL.Create;
			commands["imul"] = IMUL.Create;
			commands["div"] = DIV.Create;
			commands["idiv"] = IDIV.Create;

			commands["jmp"] = JMP.Create;
			commands["loop"] = LOOP.Create;
			foreach(var command in Jxx.Codes.Keys)
			{
				commands[command.ToLower()] = Jxx.Create;
			}
		}

		public static Command Create(string command, ILineInfo line)
		{
			if (!commands.ContainsKey(command))
			{
				throw new Exceptions.NotACommandException(command);
			}
			return commands[command](line);
		}

		public static List<string> List
		{
			get
			{
				return commands.Keys.ToList();
			}
		}
	}
}
