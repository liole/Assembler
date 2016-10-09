using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Arguments
{
	class MemoryName: IArgument
	{
		public string Name { get; set; }
		private MemoryManager mgr;

		public Lexer.CaptureInfo Capture { get; set; } // if not declared need to point to it

		public MemoryName(string name, Lexer.CaptureInfo capture = null)
		{
			Name = name;
			Capture = capture;
		}

		public bool Attach(MemoryManager mgr)
		{
			this.mgr = mgr;
			return mgr.Variables.ContainsKey(Name);
		}

		public void Detach()
		{
			this.mgr = null;
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

		public bool IsWord
		{
			get
			{
				return mgr.VariableTypes[Name] == Definition.DefinitionType.Word;
			}
		}
	}
}
