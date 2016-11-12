using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assembler.Logic.Arguments;

namespace Assembler.Logic.Commands
{
	class CALL: Command
	{
		public string ProcedureName { get; set; }
		public Lexer.CaptureInfo Capture { get; set; }

		byte[] assemble(MemoryManager mgr)
		{
			byte cmd = 0xe8;
			if (!mgr.IsProcedureDecalared(ProcedureName))
			{
				throw new Exceptions.ProcedureNotDeclaredException(ProcedureName, "CALL", Capture);
			}
			Int16 from = mgr.Pointer;
			Int16 to = mgr.Procedures[ProcedureName];
			var shift = (Int16)(to - (from + 3));
			return new byte[] { cmd, (byte)shift, (byte)(shift >> 8) };
		}

		public static CALL Create(ILineInfo line)
		{
			if (line.NumberOfArguments != 1)
			{
				throw new Exceptions.ArgumentNumberException("CALL", line.NumberOfArguments);
			}
			var cmd = new CALL()
			{
				LineNumber = line.LineNumber
			};
			switch (line.TypeOfArgument(1))
			{
				case Lexer.ArgumentType.Name:
					cmd.ProcedureName = line.Argument(1).ToLower();
					cmd.Capture = line.LastCapture;
					cmd.Assemble = cmd.assemble;
					break;
			}
			if (cmd.Assemble == null)
			{
				throw new Exceptions.ArgumentException("CALL", line.TypeOfArgument(1));
			}
			return cmd;
		}
	}
}
