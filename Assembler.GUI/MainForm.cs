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

		private void MainForm_Load(object sender, EventArgs e)
		{
			compile();
		}

		private bool compile()
		{
			var text = editor.Text;
			compiler.Compile(text);
			codeLines = compiler.Program.CodeLines;
			markErros();
			editor.Invalidate();
			return compiler.Exceptions.Count == 0;
		}

		private bool build()
		{
			if (compile())
			{
				System.IO.File.WriteAllBytes("PGM.COM", compiler.Program.Code);
				return true;
			}
			return false;
		}

		private void run()
		{
			if (build())
			{
				System.IO.File.WriteAllText("PGM.BAT",
@"@echo off
CLS
PGM
PAUSE
EXIT"
				);
				System.Diagnostics.Process.Start(@"C:\Program Files (x86)\DOSBox-0.74\dosbox", "PGM.BAT -noconsole");
			}
		}

		private void drawCodeBox(PaintLineEventArgs e)
		{
			var firstLine = editor.VisibleRange.Count() == 0 ? -1 : editor.VisibleRange.First().iLine;
			if (firstLine == -1 || e.LineIndex == firstLine)
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

		private void editor_TextChanged(object sender, TextChangedEventArgs e)
		{
			e.ChangedRange.ClearStyle(commentStyle, stringStyle, keywordStyle, registerStyle, numberStyle);

			e.ChangedRange.SetStyle(commentStyle, ";.*$", RegexOptions.Multiline);
			e.ChangedRange.SetStyle(stringStyle, Lexer.STRING_LITERAL);
			e.ChangedRange.SetStyle(keywordStyle, "\\b(" + Lexer.COMMAND_LIST + "|db|dw)\\b", RegexOptions.IgnoreCase);
			e.ChangedRange.SetStyle(registerStyle, "\\b" + Lexer.REGISTER + "\\b", RegexOptions.IgnoreCase);
			e.ChangedRange.SetStyle(numberStyle, "\\b" + Lexer.NUMBER + "\\b", RegexOptions.IgnoreCase);

		}

		private int oldLine = 0;
		private void editor_SelectionChanged(object sender, EventArgs e)
		{
			var newLine = editor.Selection.FromLine;
			if (oldLine != newLine)
			{
				compile();
			}
			oldLine = newLine;
		}

		private void markErros()
		{
			editor.Range.ClearStyle(errorlineStyle);
			foreach(var e in compiler.Exceptions)
			{
				var capture = e.CaptureInfo;
				Range range = null;
				if (e.CaptureInfo.Length == 0)
				{
					range = editor.GetLine(capture.LineNumber);
				}
				else
				{
					range = editor.GetRange(
						new Place(capture.Index, capture.LineNumber),
						new Place(capture.Index + capture.Length, capture.LineNumber)
					);
				}
				range.SetStyle(errorlineStyle);
			}
		}

		private void editor_ToolTipNeeded(object sender, ToolTipNeededEventArgs e)
		{
			var error = compiler.Exceptions.FirstOrDefault(ex => 
				ex.CaptureInfo.LineNumber == e.Place.iLine && (
					ex.CaptureInfo.Length == 0 || (
						ex.CaptureInfo.Index <= e.Place.iChar &&
						ex.CaptureInfo.Index + ex.CaptureInfo.Length >= e.Place.iChar
					)
				)
			);
			if (error != null)
			{
				e.ToolTipTitle = error.ErrorName;
				e.ToolTipText = error.Message;
				e.ToolTipIcon = ToolTipIcon.Error;
			}
		}

		private void buildButton_Click(object sender, EventArgs e)
		{
			build();
		}

		private void runButton_Click(object sender, EventArgs e)
		{
			run();
		}

		private Point? mouse = null;
		private void editor_MouseDown(object sender, MouseEventArgs e)
		{
			if (Math.Abs(e.X - editor.LeftPadding) < 2)
			{
				mouse = e.Location;
			}
		}

		private void editor_MouseUp(object sender, MouseEventArgs e)
		{
			mouse = null;
		}

		private void editor_MouseMove(object sender, MouseEventArgs e)
		{
			if (mouse != null)
			{
				var oldPos = ((Point)mouse).X;
				var newPos = e.X;
				var delta = newPos - oldPos;
				editor.LeftPadding += delta;

				// update line number position
				editor.ShowLineNumbers = false;
				editor.ShowLineNumbers = true;

				mouse = e.Location;
			}
		}
	}
}
