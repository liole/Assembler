﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	public class MemoryManager
	{
		public Int16 Pointer { get; private set; }
		public Dictionary<string, Int16> Labels { get; private set; }
		public Dictionary<string, Int16> Variables { get; private set; }
		public Dictionary<string, DefinitionType> VariableTypes { get; private set; }
		public Dictionary<string, Int16> Procedures { get; private set; }
		public Dictionary<string, Int16> ProcedureEnds { get; private set; }
		public Dictionary<string, bool> ProcedureReturns { get; set; }

		private List<string> currentProcedures = new List<string>();

		public static Int16 UndefinedAddress = -1; //0xffff;

		public MemoryManager()
		{
			Pointer = 0;
			Labels = new Dictionary<string, Int16>();
			Variables = new Dictionary<string, Int16>();
			VariableTypes = new Dictionary<string, DefinitionType>();
			Procedures = new Dictionary<string, Int16>();
			ProcedureEnds = new Dictionary<string, Int16>();
			ProcedureReturns = new Dictionary<string, bool>();
		}

		public Int16 MovePointer(Int16 offset)
		{
			Pointer += offset;
			return Pointer;
		}

		public Int16 ResetPointer(Int16 value = 0)
		{
			Pointer = value;
			return Pointer;
		}

		public void DeclareLabel(string name)
		{
			Labels[name] = UndefinedAddress;
		}

		public void DeclareVariable(string name, DefinitionType type)
		{
			Variables[name] = UndefinedAddress;
			VariableTypes[name] = type;
		}

		public void DeclareProcedure(string name)
		{
			Procedures[name] = UndefinedAddress;
		}

		public void EndProcedureDeclaration(string name)
		{
			ProcedureEnds[name] = UndefinedAddress;
		}

		public bool IsLabelDecalared(string name)
		{
			return Labels.ContainsKey(name);
		}

		public bool IsVariableDecalared(string name)
		{
			return Variables.ContainsKey(name);
		}

		public bool IsProcedureDecalared(string name)
		{
			return Procedures.ContainsKey(name);
		}

		public bool IsProcedureEnded(string name)
		{
			return ProcedureEnds.ContainsKey(name);
		}

		public bool DoesProcedureReturn(string name)
		{
			return ProcedureReturns.ContainsKey(name);
		}

		public Int16 DefineLabel(string name)
		{
			Labels[name] = Pointer;
			return Pointer;
		}

		public Int16 DefineVariable(string name)
		{
			Variables[name] = Pointer;
			return Pointer;
		}

		public Int16 DefineProcedure(string name)
		{
			Procedures[name] = Pointer;
			currentProcedures.Add(name);
			return Pointer;
		}

		public Int16 EndProcedureDefinition(string name)
		{
			ProcedureEnds[name] = Pointer;
			currentProcedures.RemoveAt(currentProcedures.Count - 1);
			return Pointer;
		}

		public void ReturnFromProcedure(string name)
		{
			ProcedureReturns[name] = true;
		}

		public void ReturnFromProcedure()
		{
			if (currentProcedures.Count != 0)
			{
				ReturnFromProcedure(currentProcedures.Last());
			}
		}
	}
}
