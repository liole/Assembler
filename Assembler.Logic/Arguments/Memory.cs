using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Arguments
{
	abstract class Memory: IArgument
	{
		protected MemoryManager mgr;
		public virtual string Name { get; private set; }

		public Lexer.CaptureInfo Capture { get; set; } // if not declared need to point to it

		public Memory(Lexer.CaptureInfo capture = null, string name = null)
		{
			Capture = capture;
			Name = name;
		}

		public virtual bool Attach(MemoryManager mgr)
		{
			this.mgr = mgr;
			return true;
		}

		public virtual void Detach()
		{
			this.mgr = null;
		}

		public abstract bool IsWord { get; }
		public abstract byte MOD { get; }
		public abstract byte RM { get; }

		public abstract byte[] GetData(bool word = true);
	}
}
