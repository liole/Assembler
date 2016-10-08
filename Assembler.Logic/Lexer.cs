﻿using System;
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
		public int LineNumber { get; set; }

		static Lexer()
		{
			regex = new Regex(LINE, RegexOptions.Compiled|RegexOptions.IgnoreCase);
		}

		public Lexer(string text)
		{
			this.reader = new StringReader(text);
			LineNumber = 0;
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
				throw new Exceptions.FormatException(LineNumber);
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
				return group.Success ? group.Value.ToLower() : null;
			}
		}

		public string Command
		{
			get
			{
				var group = match.Groups["command"];
				return group.Success ? group.Value.ToLower() : null;
			}
		}

		public string Definition
		{
			get
			{
				var group = match.Groups["definition"];
				return group.Success ? group.Value.ToLower() : null;
			}
		}

		public string Comment
		{
			get
			{
				var group = match.Groups["comment"];
				return group.Success ? group.Value : null;
			}
		}

		public int NumberOfArguments
		{
			get
			{
				var args = match.Groups["argument"].Captures;
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
			return group.Value;
		}

		private bool isGroupIn(Capture capt, string name)
		{
			return match.Groups[name].Captures.OfType<Capture>().Count(c =>
				c.Index >= capt.Index &&
				c.Index + c.Length <= capt.Index + capt.Length) > 0;
		}

		private Capture getGroupInside(Capture capt, string name)
		{
			return match.Groups[name].Captures.OfType<Capture>().FirstOrDefault(c =>
				c.Index >= capt.Index &&
				c.Index + c.Length <= capt.Index + capt.Length);
		}

		public ArgumentType TypeOfArgument(int n)
		{
			var args = match.Groups["argument"].Captures;
			if (n > args.Count)
			{
				return ArgumentType.None;
			}
			var group = args[n-1];
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
				throw new Exceptions.OverflowException(LineNumber, capture.Value);
			}
			return null;
		}

		public Int16? ArgumentAsNumber(int n)
		{
			var args = match.Groups["argument"].Captures;
			if (n > args.Count)
			{
				return null;
			}
			var group = args[n-1];
			return groupAsNumber(group);
		}

		public Int16? ValueAsNumber()
		{
			var group = match.Groups["value"];
			return groupAsNumber(group);
		}

		public string ValueAsString()
		{
			var group = match.Groups["value"];
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
			None, Register, Number, Name
		}

		public enum ValueType
		{
			None, Number, String
		}


		public static string DEC_NUMBER = "(?<number_dec>\\-?[0-9]+)d?";
		public static string BIN_NUMBER = "(?<number_bin>[01]+)b";
		public static string HEX_NUMBER_1 = "(?<number_hex>[0-9][0-9a-f]*)h";
		public static string HEX_NUMBER_2 = "0x(?<number_hex>[0-9a-f]+)";
		public static string HEX_NUMBER = $"{HEX_NUMBER_1}|{HEX_NUMBER_2}";
		public static string NUMBER = $"(?<number>{BIN_NUMBER}|{HEX_NUMBER}|{DEC_NUMBER})";
		public static string STRING_LITERAL_1 = "'(?<string_literal>[^']*)'";
		public static string STRING_LITERAL_2 = "\"(?<string_literal>[^\"]*)\"";
		public static string STRING_LITERAL = $"{STRING_LITERAL_1}|{STRING_LITERAL_2}";
		public static string LITERAL = $"(?<literal>{STRING_LITERAL}|{NUMBER})";
		public static string REGISTER = "(?<register>ax|bx|cx|dx|sp|bp|si|di|al|bl|cl|dl|ah|bh|ch|dh)";
		public static string NAME = "(?<name>[a-z_][a-z_0-9]*)";
		public static string COMMAND_LIST = String.Join("|", Commands.Creator.List);
		public static string COMMAND = $"(?<command>[a-z]+)";
		public static string DEFINITION = "(?<definition>db|dw)";
		public static string DEFINE_LINE = $"(?:{NAME}\\s+)?{DEFINITION}\\s+(?<value>{LITERAL}|\\?)";
		public static string ARGUMENT = $"{REGISTER}|{NUMBER}|{NAME}";
		public static string COMMAND_LINE = $"{COMMAND}(?:\\s+(?<argument>{ARGUMENT}))?(?:\\s*,\\s*(?<argument>{ARGUMENT}))*";
		public static string LINE = $"^\\s*(?:(?<label>{NAME})\\s*:)?\\s*(?:{DEFINE_LINE}|{COMMAND_LINE})?\\s*(?:;(?<comment>.*))?$";
	
	}
}
