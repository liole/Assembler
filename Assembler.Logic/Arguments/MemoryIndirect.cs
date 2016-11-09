using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Arguments
{
	class MemoryIndirect: Memory
	{
		public string[] Registers { get; set; }
		public Int16?[] Numbers { get; set; }

		public MemoryIndirect(string name, IEnumerable<string> registers, IEnumerable<Int16?> numbers, Lexer.CaptureInfo capture = null):
			base(capture, name)
		{
			Registers = registers.Select(r => r.ToUpper()).OrderBy(r => r).ToArray();
			Numbers = numbers.ToArray();
			CheckRegisters();
		}

		public static Dictionary<string, byte> Codes { get; set; }

		static MemoryIndirect()
		{
			Codes = new Dictionary<string, byte>();
			Codes["BX+SI"] = 0x00;
			Codes["BX+DI"] = 0x01;
			Codes["BP+SI"] = 0x02;
			Codes["BP+DI"] = 0x03;

			Codes["SI"] = 0x04;
			Codes["BI"] = 0x05;
			Codes["BP"]  = Codes[""] = 0x06;
			Codes["BX"] = 0x07;
		}

		public void CheckRegisters()
		{
			if (Registers.Count() > 2)
			{
				throw new Exceptions.TooManyRegistersException(Registers.Count(), Capture);
			}
			if (!Codes.ContainsKey(RegisterCombination))
			{
				throw new Exceptions.RegisterCombinationNotSupportedException(RegisterCombination, Capture);
			}
		}

		public override bool Attach(MemoryManager mgr)
		{
			base.Attach(mgr);
			if (Name == null)
			{
				return true;
			}
			return mgr.Variables.ContainsKey(Name);
		}

		public string RegisterCombination
		{
			get { return String.Join("+", Registers); }
		}

		public byte Code
		{
			get { return Codes[RegisterCombination]; }
		}

		public Int16 GetShift()
		{
			Int16 addr = (Int16)Numbers.Sum(n => (Int16)n);	// Warn if overflow ?
			if (Name != null)
			{
				addr += mgr.Variables[Name];
			}
			return addr;
		}

		public override bool IsWord
		{
			get
			{
				if (Name == null)
				{
					return true;
				}
				return mgr.VariableTypes[Name] == Definition.DefinitionType.Word;
			}
		}

		public bool IsDirect
		{
			get { return Name != null && RegisterCombination == ""; }
		}

		// size of shift/address
		public override byte MOD
		{
			get
			{
				if  (IsDirect)
				{
					return 0;
				}
				if (Name != null)
				{
					return 2;
				}
				var addr = GetShift();
				if (addr == 0 && RegisterCombination != "BP") // [BP] := [BP+0], because code for [BP] == direct
				{
					return 0;
				}
				if (addr >> 8 == 0)
				{
					return 1;
				}
				return 2;
			}
		}
		public override byte RM
		{
			get { return Code; }
		}

		public override byte[] GetData(bool word = true)
		{
			var addr = GetShift();
			if (MOD == 0 && !IsDirect)
			{
				return new byte[] { };
			}
			if (MOD == 1)
			{
				return new byte[] { (byte)addr };
			}
			return new[] { (byte)addr, (byte)(addr >> 8) };
		}
	}
}
