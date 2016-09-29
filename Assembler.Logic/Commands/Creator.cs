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
		}

		public static Command Create(string command, ILineInfo line)
		{
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
