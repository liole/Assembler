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

		public static string[] Types = new[] { "None", "Register", "Number", "Memory", "Indirect memory" };

		public ArgumentException(string commandName,
			Lexer.ArgumentType arg1Type, Lexer.ArgumentType arg2Type = Lexer.ArgumentType.None,
			Lexer.CaptureInfo capture = null):
			base(commandName, capture)
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
					return String.Format("Command '{0}' can not have argument of type '{1}'.",
						CommandName, arg1);
				}
				else
				{
					return String.Format("Combination of arguments: '{0}', '{1}' for command '{2}' is not supported.",
						arg1, arg2, CommandName);
				}
			}
		}

		public override string ErrorName
		{
			get
			{
				return "Argument error";
			}
		}
	}
}
