using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	class Definition: IInstruction
	{
		public int LineNumber { get; set; }
		public string Name { get; set; }
		public DefinitionType Type { get; set; }
		public byte[] Value { get; set; }

		public byte[] Assemble(MemoryManager mgr)
		{
			if (Name != null)
			{
				mgr.Variables[Name] = mgr.Pointer;
			}
			return Value;
		}

		public static Definition Create(ILineInfo line)
		{
			var def = new Definition()
			{
				LineNumber = line.LineNumber
			};
			def.Name = line.GetName();
			switch (line.Definition)
			{
				case "db":
					def.Type = DefinitionType.Byte;
					break;
				case "dw":
					def.Type = DefinitionType.Word;
					break;
			}
			switch (line.TypeOfValue())
			{
				case Lexer.ValueType.Number:
					var arg = new Arguments.Number((Int16)line.ValueAsNumber());
					if (arg.IsWord && def.Type == DefinitionType.Byte)
					{
						throw new Exceptions.ArgumentSizeException(line.LineNumber, line.Definition);
					}
					def.Value = arg.GetValue(def.Type == DefinitionType.Word);
					break;
				case Lexer.ValueType.String:
					var val = line.ValueAsString();
					while (val.Length < (int)def.Type + 1)
					{
						val += '\0';
					}
					def.Value = line.ValueAsString().Select(c => (byte)c).ToArray();
					break;
				case Lexer.ValueType.None:
					switch (def.Type)
					{
						case DefinitionType.Byte:
							def.Value = new byte[] { 0x00 };
							break;
						case DefinitionType.Word:
							def.Value = new byte[] { 0x00, 0x00 };
							break;
					}
					break;
			}
			return def;
		}

		public enum DefinitionType
		{
			Byte, Word
		}
	}
}
