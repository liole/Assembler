using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Assembler.Logic;

namespace Assembler.GUI
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var compiler = new Compiler();
			var text = textBox1.Text;
			var code = compiler.Compile(text);
			var result = String.Join(" ", code.Select(b => b.ToString("X2")));
			textBox2.Text = result;
		}
	}
}
