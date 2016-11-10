using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Arguments
{
	class MemoryName: Memory
	{
		public MemoryName(string name, Lexer.CaptureInfo capture = null):
			base(capture, name)
		{
		}

		public override bool Attach(MemoryManager mgr)
		{
			base.Attach(mgr);
			return mgr.IsVariableDecalared(Name);
		}

		public Int16 GetAddress()
		{
			return mgr.Variables[Name];
		}

		public byte[] GetReverseAddress()
		{
			var addr = GetAddress();
			return new[] { (byte)addr, (byte)(addr >> 8) };
		}

		public override bool IsWord
		{
			get
			{
				return mgr.VariableTypes[Name] == Definition.DefinitionType.Word;
			}
		}

		public override byte MOD
		{
			get { return 0x00; }
		}
		public override byte RM
		{
			get { return 0x06; }
		}

		public override byte[] GetData(bool word = true)
		{
			return GetReverseAddress();
		}
	}
}
