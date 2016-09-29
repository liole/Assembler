using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Logic
{
	class Label: Instruction
	{
		public string Name { get; set; }

		public Label(string name)
		{
			Name = name;
		}
	}
}
