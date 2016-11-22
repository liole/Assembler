using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assembler.GUI
{
	partial class AboutBox : Form
	{
		public AboutBox()
		{
			InitializeComponent();
		}

		private void urlLabel_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://github.com/liole/Assembler");
		}
	}
}
