using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	interface ILineInfo
	{
		int LineNumber { get; set; }
		Lexer.LineType Type { get; }

		bool HasLabel { get; }
		string Label { get; }
		string Command { get; }
		string Definition { get; }
		string Comment { get; }

		int NumberOfArguments { get; }
		string Argument(int n);
		Lexer.ArgumentType TypeOfArgument(int n);
		Lexer.ValueType TypeOfValue();


		Int16? ArgumentAsNumber(int n);
		Int16? ValueAsNumber();
		string ValueAsString();

	}
}
