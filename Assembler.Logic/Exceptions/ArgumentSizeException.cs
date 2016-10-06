﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Exceptions
{
	public class ArgumentSizeException: CommandException
	{
		public ArgumentSizeException(int lineNumber, string commandName):
			base(lineNumber, commandName)
		{
		}

		public override string Message
		{
			get
			{
				return String.Format("Size of argument(s) for command '{0}' is incompatible.[Line {1}]",
					CommandName, LineNumber);
			}
		}
	}
}
