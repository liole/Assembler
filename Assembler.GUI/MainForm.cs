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
			//System.Diagnostics.Process.Start(@"C:\nasm16\start.cmd");
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			textBox1.Text = System.IO.File.ReadAllText(@"C:\nasm16\test.asm");
			button1.PerformClick();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			var compiler = new Compiler();
			var text = textBox1.Text;
			var code = compiler.Compile(text);
			System.IO.File.WriteAllBytes("PGM.COM", code);
			System.IO.File.WriteAllText("PGM.BAT", @"
@echo off
CLS
PGM
PAUSE
EXIT
			");
			System.Diagnostics.Process.Start(@"C:\Program Files (x86)\DOSBox-0.74\dosbox","PGM.BAT -noconsole");
		}
	}
}
