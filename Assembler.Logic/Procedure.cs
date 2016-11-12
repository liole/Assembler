using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	class Procedure: IInstruction
	{
		public int LineNumber { get; set; }
		public string Name { get; set; }
		public bool End { get; set; }
		public bool Inline { get; set; }

		public byte[] Assemble(MemoryManager mgr)
		{
			var result = new byte[] { };
			if (End)
			{
				mgr.EndProcedureDefinition(Name);
			}
			else
			{
				if (Inline && mgr.IsProcedureEnded(Name))
				{
					result = Commands.JMP.Code(mgr.Pointer, mgr.ProcedureEnds[Name]);
				}
				// skip part of procedure declaration
				mgr.MovePointer((Int16)result.Length);
				mgr.DefineProcedure(Name);
				mgr.MovePointer((Int16)(-result.Length));

				if (!mgr.IsProcedureEnded(Name))
				{
					throw new Exceptions.ProcedureNotEndedException(Name);
				}
			}
			return result;
		}

		public static Procedure Create(ILineInfo line)
		{
			var procedure = new Procedure()
			{
				LineNumber = line.LineNumber,
				Name = line.GetName(),
				Inline = line.GetAttribute() == "inline"
			};
			switch(line.Type)
			{
				case Lexer.LineType.Procedure:
					procedure.End = false;
					break;
				case Lexer.LineType.EndP:
					procedure.End = true;
					break;
				default:
					return null;
			}
			return procedure;
		}
	}
}
