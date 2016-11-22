using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	interface ILineInfo
	{
		int LineNumber { get; }
		Lexer.LineType Type { get; }
		Lexer.CaptureInfo LastCapture { get; }

		bool HasLabel { get; }
		string Label { get; }
		string Command { get; }
		string Definition { get; }
		string Directive { get; }
		string Comment { get; }

		int NumberOfArguments { get; }
		string Argument(int n);
		Lexer.ArgumentType TypeOfArgument(int n);
		Lexer.ValueType TypeOfValue();

		string NameInArgument(int n);
		IEnumerable<string> RegistersInArgument(int n);
		IEnumerable<Int16?> NumbersInArgument(int n);


		Int16? ArgumentAsNumber(int n);
		Int16? ValueAsNumber();
		string ValueAsString();

		string GetName();
		string GetAttribute();

	}
}
