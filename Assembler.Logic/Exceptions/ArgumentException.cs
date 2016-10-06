using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class ArgumentException: CommandException
	{
		public Lexer.ArgumentType Argument1Type { get; set; }
		public Lexer.ArgumentType Argument2Type { get; set; }

		public static string[] Types = new[] { "None", "Register", "Number", "Memory" };

		public ArgumentException(int lineNumber, string commandName,
			Lexer.ArgumentType arg1Type, Lexer.ArgumentType arg2Type = Lexer.ArgumentType.None):
			base(lineNumber, commandName)
		{
			Argument1Type = arg1Type;
			Argument2Type = arg2Type;
		}

		public override string Message
		{
			get
			{
				var arg1 = Types[(int)Argument1Type];
				var arg2 = Types[(int)Argument2Type];
				if (Argument2Type == Lexer.ArgumentType.None)
				{
					return String.Format("Command '{0}' can not have argument of type '{1}'.[Line {2}]",
						CommandName, arg1, LineNumber);
				}
				else
				{
					return String.Format("Combination of arguments: '{0}', '{1}' for command '{2}' is not supported.[Line {3}]",
						arg1, arg2, CommandName, LineNumber);
				}
			}
		}
	}
}
