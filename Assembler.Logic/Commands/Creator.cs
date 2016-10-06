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
			commands["mov"] = MOV.Create;
			commands["org"] = ORG.Create;
			commands["int"] = INT.Create;
			commands["lea"] = LEA.Create;
		}

		public static Command Create(string command, ILineInfo line)
		{
			if (!commands.ContainsKey(command))
			{
				throw new Exceptions.NotACommandException(line.LineNumber, command);
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
