using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	public class Lexer: IDisposable, ILineInfo
	{
		private static Regex regex;
		private StringReader reader;
		private Match match;
		private Capture lastCapture;

		public event EventHandler<Exceptions.ExceptionHandlerArgs> ExceptionHandler;
		public int LineNumber { get; set; }
		public CaptureInfo LastCapture
		{
			get
			{
				return new CaptureInfo()
				{
					LineNumber = LineNumber,
					Index = lastCapture.Index,
					Length = lastCapture.Length
				};
			}
		}

		static Lexer()
		{
			regex = new Regex(LINE, RegexOptions.Compiled|RegexOptions.IgnoreCase);
		}

		public Lexer(string text)
		{
			this.reader = new StringReader(text);
			LineNumber = -1;
		}

		public bool NextLine()
		{
			string line = reader.ReadLine();
			if (line == null)
			{
				return false;
			}
			LineNumber++;
			match = regex.Match(line);
			if (!match.Success)
			{
				lastCapture = match;
				var exception = new Exceptions.FormatException(LastCapture);
				if (ExceptionHandler != null)
				{
					
					ExceptionHandler(this, new Exceptions.ExceptionHandlerArgs(exception));
				}
				else
				{
					throw exception;
				}
			}
			return true;
		}

		public LineType Type
		{
			get
			{
				if (match.Groups["command"].Success)
				{
					return LineType.Command;
				}
				if (match.Groups["definition"].Success)
				{
					return LineType.Definition;
				}
				return LineType.None;
			}
		}

		public bool HasLabel
		{
			get
			{
				return match.Groups["label"].Success;
			}
		}

		public string Label
		{
			get
			{
				var group = match.Groups["label"];
				lastCapture = group;
				return group.Success ? group.Value.ToLower() : null;
			}
		}

		public string Command
		{
			get
			{
				var group = match.Groups["command"];
				lastCapture = group;
				return group.Success ? group.Value.ToLower() : null;
			}
		}

		public string Definition
		{
			get
			{
				var group = match.Groups["definition"];
				lastCapture = group;
				return group.Success ? group.Value.ToLower() : null;
			}
		}

		public string Comment
		{
			get
			{
				var group = match.Groups["comment"];
				lastCapture = group;
				return group.Success ? group.Value : null;
			}
		}

		public int NumberOfArguments
		{
			get
			{
				var args = match.Groups["argument"].Captures;
				lastCapture = args.OfType<Capture>().LastOrDefault();
				if (lastCapture == null)
				{
					lastCapture = match.Groups["command"];
				}
				return args.Count;
			}
		}

		public string Argument(int n)
		{
			var args = match.Groups["argument"].Captures;
			if (n > args.Count)
			{
				return null;
			}
			var group = args[n-1];
			lastCapture = group;
			return group.Value;
		}

		private bool isGroupIn(Capture capt, string name)
		{
			return match.Groups[name].Captures.OfType<Capture>().Count(c =>
				c.Index >= capt.Index &&
				c.Index + c.Length <= capt.Index + capt.Length) > 0;
		}

		private IEnumerable<Capture> getGroupsInside(Capture capt, string name)
		{
			return match.Groups[name].Captures.OfType<Capture>().Where(c =>
				c.Index >= capt.Index &&
				c.Index + c.Length <= capt.Index + capt.Length);
		}

		private Capture getGroupInside(Capture capt, string name)
		{
			return getGroupsInside(capt, name).FirstOrDefault();
		}

		public ArgumentType TypeOfArgument(int n)
		{
			var args = match.Groups["argument"].Captures;
			if (n > args.Count)
			{
				return ArgumentType.None;
			}
			var group = args[n-1];
			lastCapture = group;
			if (isGroupIn(group, "indirect"))
			{
				return ArgumentType.Indirect;
			}
			if (isGroupIn(group, "number"))
			{
				return ArgumentType.Number;
			}
			if (isGroupIn(group, "register"))
			{
				return ArgumentType.Register;
			}
			if (isGroupIn(group, "name"))
			{
				return ArgumentType.Name;
			}
			return ArgumentType.None; // should never get here
		}

		public ValueType TypeOfValue()
		{
			var group = match.Groups["value"];
			lastCapture = group;
			if (group.Value == "?")
			{
				return ValueType.None;
			}
			if (isGroupIn(group, "number"))
			{
				return ValueType.Number;
			}
			if (isGroupIn(group, "string_literal"))
			{
				return ValueType.String;
			}
			return ValueType.None; // should never get here
		}

		private Int16? groupAsNumber(Capture group)
		{
			if (!isGroupIn(group, "number"))
			{
				return null;
			}
			Capture capture = null;
			try
			{
				if ((capture = getGroupInside(group, "number_dec")) != null)
				{
					return Convert.ToInt16(capture.Value, 10);
				}
				if ((capture = getGroupInside(group, "number_bin")) != null)
				{
					return Convert.ToInt16(capture.Value, 2);
				}
				if ((capture = getGroupInside(group, "number_hex")) != null)
				{
					return Convert.ToInt16(capture.Value, 16);
				}
			}
			catch (OverflowException)
			{
				throw new Exceptions.OverflowException(capture.Value, LastCapture); // need LastCapture here?
			}
			return null;
		}

		public string NameInArgument(int n)
		{
			var args = match.Groups["argument"].Captures;
			if (n > args.Count)
			{
				return null;
			}
			var group = args[n-1];
			var nameGroup = getGroupInside(group, "name");
			lastCapture = nameGroup ?? group;
			return nameGroup != null ? nameGroup.Value : null;
		}

		public IEnumerable<string> RegistersInArgument(int n)
		{
			var args = match.Groups["argument"].Captures;
			if (n > args.Count)
			{
				return null;
			}
			var group = args[n-1];
			var registerGroups = getGroupsInside(group, "register");
			lastCapture = group;
			return registerGroups.Select(g => g.Value);
		}

		public IEnumerable<Int16?> NumbersInArgument(int n)
		{
			var args = match.Groups["argument"].Captures;
			if (n > args.Count)
			{
				return null;
			}
			var group = args[n-1];
			var numberGroups = getGroupsInside(group, "number");
			lastCapture = group;
			return numberGroups.Select(g => groupAsNumber(g));
		}

		public Int16? ArgumentAsNumber(int n)
		{
			var args = match.Groups["argument"].Captures;
			if (n > args.Count)
			{
				return null;
			}
			var group = args[n-1];
			lastCapture = group;
			return groupAsNumber(group);
		}

		public Int16? ValueAsNumber()
		{
			var group = match.Groups["value"];
			lastCapture = group;
			return groupAsNumber(group);
		}

		public string ValueAsString()
		{
			var group = match.Groups["value"];
			lastCapture = group;
			Capture capture = getGroupInside(group, "string_literal");
			if (capture == null)
			{
				return null;
			}
			return capture.Value;
		}

		public string GetName()
		{
			var group = match.Groups["name"];
			lastCapture = group;
			return group.Success ? group.Value.ToLower() : null;
		}

		public void Dispose()
		{
			reader.Dispose();
		}

		public enum LineType
		{
			None, Command, Definition
		}

		public enum ArgumentType
		{
			None, Register, Number, Name, Indirect
		}

		public enum ValueType
		{
			None, Number, String
		}

		public class CaptureInfo
		{
			public int LineNumber { get; set; }
			public int Index { get; set; }
			public int Length { get; set; }
		}


		public static string DEC_NUMBER = "(?<number_dec>\\-?[0-9]+)d?";
		public static string BIN_NUMBER = "(?<number_bin>[01]+)b";
		public static string HEX_NUMBER_1 = "(?<number_hex>[0-9][0-9a-f]*)h";
		public static string HEX_NUMBER_2 = "0x(?<number_hex>[0-9a-f]+)";
		public static string HEX_NUMBER = $"{HEX_NUMBER_1}|{HEX_NUMBER_2}";
		public static string NUMBER = $"(?<number>{BIN_NUMBER}|{HEX_NUMBER}|{DEC_NUMBER})";
		public static string STRING_LITERAL_1 = "'(?<string_literal>[^'\\n\\r]*)'";
		public static string STRING_LITERAL_2 = "\"(?<string_literal>[^\"\\n\\r]*)\"";
		public static string STRING_LITERAL = $"{STRING_LITERAL_1}|{STRING_LITERAL_2}";
		public static string LITERAL = $"(?<literal>{STRING_LITERAL}|{NUMBER})";
		public static string REGISTER = "(?<register>ax|bx|cx|dx|sp|bp|si|di|al|bl|cl|dl|ah|bh|ch|dh)";
		public static string REG_NUM = $"(?:{REGISTER}|{NUMBER})";
		public static string NAME = "(?<name>[a-z_][a-z_0-9]*)";
		public static string COMMAND_LIST = String.Join("|", Commands.Creator.List);
		public static string COMMAND = $"(?<command>[a-z]+)";
		public static string DEFINITION = "(?<definition>db|dw)";
		public static string DEFINE_LINE = $"(?:{NAME}\\s+)?{DEFINITION}\\s+(?<value>{LITERAL}|\\?)";
		public static string INDIRECT = $"(?<indirect>{NAME}?\\[{REG_NUM}(?:\\+{REG_NUM})*\\])";
		public static string ARGUMENT = $"(?<argument>{REGISTER}|{NUMBER}|{NAME}|{INDIRECT})";
		public static string COMMAND_LINE = $"{COMMAND}(?:\\s+{ARGUMENT})?(?:\\s*,\\s*{ARGUMENT})*";
		public static string LINE = $"^\\s*(?:(?<label>{NAME})\\s*:)?\\s*(?:{DEFINE_LINE}|{COMMAND_LINE})?\\s*(?:;(?<comment>.*))?$";
	
	}
}
