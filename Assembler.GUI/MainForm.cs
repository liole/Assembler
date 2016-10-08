using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Assembler.Logic;
using FastColoredTextBoxNS;

namespace Assembler.GUI
{
	public partial class MainForm : Form
	{
		private Compiler compiler;
		private byte[][] codeLines;

		TextStyle keywordStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
		TextStyle registerStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
		TextStyle numberStyle = new TextStyle(Brushes.DarkMagenta, null, FontStyle.Regular);
		TextStyle stringStyle = new TextStyle(Brushes.DarkOrange, null, FontStyle.Regular);
		TextStyle commentStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
		Style errorlineStyle = new WavyLineStyle(255, Color.Red);

		public MainForm()
		{
			compiler = new Compiler();
			InitializeComponent();
		}

		private void build()
		{
			var text = editor.Text;
			compiler.Compile(text);
			codeLines = compiler.Program.CodeLines;
			editor.Invalidate();
		}

		private void compile()
		{
			build();
			System.IO.File.WriteAllBytes("PGM.COM", compiler.Program.Code);
		}

		private void run()
		{
			compile();
			System.IO.File.WriteAllText("PGM.BAT", @"
@echo off
CLS
PGM
PAUSE
EXIT
			");
			System.Diagnostics.Process.Start(@"C:\Program Files (x86)\DOSBox-0.74\dosbox", "PGM.BAT -noconsole");
		}

		private void drawCodeBox(PaintLineEventArgs e)
		{
			var firstLine = editor.VisibleRange.First().iLine;
			if (e.LineIndex == firstLine)
			{
				e.Graphics.DrawLine(
					Pens.DimGray, 
					new Point(editor.LeftPadding, 0), 
					new Point(editor.LeftPadding, editor.Height)
				);
				e.Graphics.FillRectangle(
					Brushes.LightYellow,
					new Rectangle(0, 0, editor.LeftPadding, editor.Height)
				);
			}
		}

		private void editor_PaintLine(object sender, PaintLineEventArgs e)
		{
			drawCodeBox(e);

			if (e.LineIndex >= codeLines.Length)
			{
				return;
			}
			int padding = 5;
			var code = codeLines[e.LineIndex];
			var codeStr = String.Join(" ", code.Select(b => b.ToString("X2")));
			e.Graphics.DrawString(
				codeStr,
				editor.Font,
				Brushes.Gray,
				new Rectangle(padding, e.LineRect.Top, editor.LeftPadding - padding, e.LineRect.Height), 
				new StringFormat() { 
					FormatFlags = StringFormatFlags.NoWrap,
					Trimming = StringTrimming.EllipsisCharacter
				}
			);
			
		}

		private int oldLineCount = 0;
		private void editor_TextChanged(object sender, TextChangedEventArgs e)
		{
			e.ChangedRange.ClearStyle(commentStyle, stringStyle, keywordStyle, registerStyle, numberStyle);

			e.ChangedRange.SetStyle(commentStyle, ";.*$", RegexOptions.Multiline);
			e.ChangedRange.SetStyle(stringStyle, Lexer.STRING_LITERAL);
			e.ChangedRange.SetStyle(keywordStyle, "\\b(" + Lexer.COMMAND_LIST + "|db|dw)\\b", RegexOptions.IgnoreCase);
			e.ChangedRange.SetStyle(registerStyle, "\\b" + Lexer.REGISTER + "\\b", RegexOptions.IgnoreCase);
			e.ChangedRange.SetStyle(numberStyle, "\\b" + Lexer.NUMBER + "\\b", RegexOptions.IgnoreCase);

			if (oldLineCount != editor.LinesCount)
			{
				build();
			}
			oldLineCount = editor.LinesCount;
		}

		private void compileButton_Click(object sender, EventArgs e)
		{
			compile();
		}

		private void runButton_Click(object sender, EventArgs e)
		{
			run();
		}
	}
}
