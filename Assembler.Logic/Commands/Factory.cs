using Assembler.Logic.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic.Commands
{
	class Factory
	{
		public static void Swap<T>(ref T a, ref T b)
		{
			T c = a;
			a = b;
			b = c;
		}

		public static void ForEach<T>(IEnumerable<object> enumeration, Action<T> action)
			where T: class
		{
			foreach (var item in enumeration)
			{
				if (item is T)
				{
					var casted = item as T;
					action(casted);
				}
			}
		}

		public static bool CheckArgumentSize(string name, IArgument arg1, IArgument arg2)
		{
			if (arg1.IsWord == arg2.IsWord)
			{
				return arg1.IsWord;
			}
			if (arg1 is MemoryIndirect && !(arg1 as MemoryIndirect).HasKnownSize)
			{
				return arg2.IsWord;
			}
			if (arg2 is MemoryIndirect && !(arg2 as MemoryIndirect).HasKnownSize)
			{
				return arg1.IsWord;
			}
			if (arg2 is Number && arg1.IsWord)
			{
				return true;
			}
			throw new Exceptions.ArgumentSizeException(name);
		}

		public static Func<MemoryManager, byte[]> Assemble1Arg(byte cmd, string name, IArgument arg1, byte iReg = 0)
		{
			return (mgr) =>
			{
				if (arg1 is Memory)
				{
					var mem = arg1 as Memory;
					var declared = mem.Attach(mgr);
					if (!declared)
					{
						throw new Exceptions.VariableNotDeclaredException(mem.Name, name, mem.Capture);
					}
				}
				var w = arg1.IsWord;
				var cmdw = (byte)(cmd | (w ? 1 : 0));
				byte mod = (byte)arg1.MOD;
				byte reg = iReg;
				byte rm = arg1.RM;
				var modrm = Command.GetAddressingMode(mod, reg, rm);
				var data = new List<byte>() { cmdw, modrm };
				data.AddRange(arg1.GetData(w));
				if (arg1 is Memory)
				{
					(arg1 as Memory).Detach();
				}
				return data.ToArray();
			};
		}

		public static Func<MemoryManager, byte[]> Assemble2Args(byte cmd, string name, IArgument arg1, IArgument arg2, byte iReg = 0, byte? prefix = null, bool wordOnly = false)
		{
			var args = new[] { arg1, arg2 };
			return (mgr) =>
			{
				ForEach<Memory>(args, mem =>
				{
					var declared = mem.Attach(mgr);
					if (!declared)
					{
						throw new Exceptions.VariableNotDeclaredException(mem.Name, name, mem.Capture);
					}
				});
				var w = CheckArgumentSize(name, arg1, arg2);
				if (wordOnly && !w)
				{
					throw new Exceptions.ArgumentSizeException(name);
				}
				var cmdw = (byte)(cmd | (w ? 1 : 0));
				ForEach<Number>(args, num => num.RM = iReg);
				byte mod = (byte)(arg1.MOD & arg2.MOD);
				byte reg = arg2.RM;
				byte rm = arg1.RM;
				if (arg1.MOD != mod)
				{
					Swap(ref reg, ref rm);
				}
				var modrm = Command.GetAddressingMode(mod, reg, rm);
				var data = new List<byte>() { cmdw, modrm };
				ForEach<IArgument>(args, arg => data.AddRange(arg.GetData(w)));
				ForEach<Memory>(args, mem => mem.Detach());
				if (prefix != null)
				{
					data.Insert(0, (byte)prefix);
				}
				return data.ToArray();
			};
		}
	}
}
