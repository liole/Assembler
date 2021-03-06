﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Arguments
{
	public interface IArgument
	{
		bool IsWord { get; }
		byte MOD { get; }
		byte RM { get; }

		byte[] GetData(bool word = true);
	}
}
