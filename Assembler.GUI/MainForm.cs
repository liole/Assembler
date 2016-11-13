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
		private string fileName = null;
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
			updateTitle();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			compile();
			splitterSymbols.Visible = false; // doesn't work if hidden in constructor
		}

		private bool compile()
		{
			var text = editor.Text;
			var code = compiler.Compile(text);
			codeLines = compiler.Program.CodeLines;
			setProgramInfo(code, text);
			showErrors();
			showSymbols();
			markProceduresFoldable();
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
				e.Graphics.FillRectangle(
					Brushes.LightYellow,
					new Rectangle(0, 0, editor.LeftPadding, editor.Height)
				);
				e.Graphics.DrawLine(
					new Pen(Color.DimGray, resizeCodeAreaHover ? 5 : 1),
					new Point(editor.LeftPadding, 0),
					new Point(editor.LeftPadding, editor.Height)
				);
			}
		}

		private void editor_PaintLine(object sender, PaintLineEventArgs e)
		{
			drawCodeBox(e);

			int padding = 5;
			if (e.LineIndex >= codeLines.Length || editor.LeftPadding <= padding)
			{
				return;
			}
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

			e.ChangedRange.SetStyle(stringStyle, Lexer.STRING_LITERAL);
			e.ChangedRange.SetStyle(commentStyle, ";.*$", RegexOptions.Multiline);
			e.ChangedRange.SetStyle(keywordStyle, "\\b(" + Lexer.COMMAND_LIST + "|db|dw|proc|endp|inline)\\b", RegexOptions.IgnoreCase);
			e.ChangedRange.SetStyle(registerStyle, "\\b" + Lexer.REGISTER + "\\b", RegexOptions.IgnoreCase);
			e.ChangedRange.SetStyle(numberStyle, "\\b" + Lexer.NUMBER + "\\b", RegexOptions.IgnoreCase);

			updateTitle();
		}

		private int oldLine = 0;
		private void editor_SelectionChanged(object sender, EventArgs e)
		{
			if (!editor.Selection.IsEmpty) return;
			var cursor = editor.Selection.Start;
			if (cursor != null)
			{
				cursorPosLabel.Text = String.Format("{0}:{1}", cursor.iLine+1, cursor.iChar+1);
			}
			else
			{
				cursorPosLabel.Text = "--:--";
			}
			var newLine = editor.Selection.FromLine;
			if (oldLine != newLine)
			{
				compile();
			}
			oldLine = newLine;
		}

		private void showErrors()
		{
			markErrors();
			int number = 0;
			var errorData = compiler.Exceptions.Select(e => new ErrorInfo()
			{ 
				No = ++number,
				Line = e.LineNumber + 1,
				Name = e.ErrorName,
				Description = e.Message
			});
			errorsTable.DataSource = errorData.ToArray();
			errorsTable.AutoResizeColumn(0, DataGridViewAutoSizeColumnMode.AllCells);
			errorsTable.AutoResizeColumn(1, DataGridViewAutoSizeColumnMode.AllCells);
			errorsTable.AutoResizeColumn(2, DataGridViewAutoSizeColumnMode.AllCells);
			errorPanelTitleLabel.Text = String.Format("Errors ({0})", number);
			errorsCountLabel.Text = number == 0
				? "No errors"
				: String.Format("{0} error(s)", number);
			errorsCountLabel.BackColor = number == 0
				? Color.Transparent
				: Color.Tomato;
			errorsTable.CurrentCell = null;
		}

		private void markErrors()
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

		private void showSymbols()
		{
			symbolsTitleLabel.Text = String.Format("Symbols ({0})", compiler.Program.SymbolsCount);
			showVariables();
			showLabels();
			showProcedures();
		}

		private void showVariables()
		{
			var variablesData = compiler.Program.VariablesInfo;
			variablesTable.DataSource = variablesData;
			variablesTable.AutoResizeColumn(0, DataGridViewAutoSizeColumnMode.AllCells);
			variablesTable.Columns[3].DefaultCellStyle.Format = "X4";
			variablesLabel.Text = String.Format("Varibales ({0})", variablesData.Count());
			variablesTable.CurrentCell = null;
		}

		private void showLabels()
		{
			var labelsData = compiler.Program.LabelsInfo;
			labelsTable.DataSource = labelsData;
			labelsTable.AutoResizeColumn(0, DataGridViewAutoSizeColumnMode.AllCells);
			labelsTable.AutoResizeColumn(1, DataGridViewAutoSizeColumnMode.AllCells);
			labelsTable.AutoResizeColumn(2, DataGridViewAutoSizeColumnMode.AllCells);
			labelsTable.Columns[2].DefaultCellStyle.Format = "X4";
			labelsLabel.Text = String.Format("Labels ({0})", labelsData.Count());
			labelsTable.CurrentCell = null;
		}

		private void showProcedures()
		{
			var proceduresData = compiler.Program.ProceduresInfo;
			proceduresTable.DataSource = proceduresData;
			proceduresTable.AutoResizeColumn(0, DataGridViewAutoSizeColumnMode.AllCells);
			proceduresTable.AutoResizeColumn(1, DataGridViewAutoSizeColumnMode.AllCells);
			proceduresTable.AutoResizeColumn(2, DataGridViewAutoSizeColumnMode.AllCells);
			proceduresTable.Columns[2].DefaultCellStyle.Format = "X4";
			proceduresLabel.Text = String.Format("Procedures ({0})", proceduresData.Count());
			proceduresTable.CurrentCell = null;
		}

		private void markProceduresFoldable()
		{
			editor.Range.ClearFoldingMarkers();
			var procedures = compiler.Program.ProcedureLines;
			foreach(var proc in procedures)
			{
				var markerName = String.Format("p{0}", proc.Name);
				editor[proc.Start].FoldingStartMarker = markerName;
				editor[proc.End].FoldingEndMarker = markerName;
			}
		}

		private void setProgramInfo(byte[] code, string text)
		{
			var pgmL = (float)code.Length;
			var pgmT = "bytes";
			//if (pgmL > 512)
			//{
			//	pgmL /= 1024;
			//	pgmT = "Kb";
			//}
			var codL = (float)text.Length;
			var codT = "bytes";
			//if (codL > 512)
			//{
			//	codL /= 1024;
			//	codT = "Kb";
			//}
			programInfoLine.Text = String.Format("Program: {0:0.#} {1}, Code {2:0.#} {3} | ratio = {4:0.##}",
				pgmL, pgmT, codL, codT, (double)text.Length / code.Length);
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

		private bool shouldResizeCodeArea(int x)
		{
			return Math.Abs(x - editor.LeftPadding) < 3;
		}

		private bool resizeCodeAreaHover = false;
		private bool mouseDown = false;
		private void editor_MouseDown(object sender, MouseEventArgs e)
		{
			if (shouldResizeCodeArea(e.X))
			{
				mouseDown = true;
				opCodeSize = editor.LeftPadding;
			}
		}

		private void editor_MouseUp(object sender, MouseEventArgs e)
		{
			mouseDown = false;
		}

		void setOpCodeSize(int size)
		{
			editor.LeftPadding = size;
			// update line number position
			editor.ShowLineNumbers = false;
			editor.ShowLineNumbers = true;

			opCodeToolStripMenuItem.Checked = size >= 1;
		}

		private void editor_MouseMove(object sender, MouseEventArgs e)
		{
			var isHover = mouseDown || shouldResizeCodeArea(e.X);
			if (resizeCodeAreaHover != isHover)
			{
				resizeCodeAreaHover = isHover;
				editor.Invalidate();
			}
			if (mouseDown)
			{
				var value = e.X;
				if (value < 0)
				{
					value = 0;
				}
				setOpCodeSize(value);				
			}
		}

		bool settingZoom = false;
		void setZoom(int value, bool editor = true, bool trackBar = true, bool label = true)
		{
			if (settingZoom)
			{
				return;
			}
			settingZoom = true;
			if (editor)
			{
				this.editor.Zoom = value;
			}
			if (trackBar)
			{
				var zoom10 = (double)value / 10;
				var pos = (Math.Log10(zoom10) - 1) * 150;
				this.zoomTrackBar.Value = (int)pos;
			}
			if (label)
			{
				this.zoomBtn.Text = String.Format("{0} %", value);
			}
			settingZoom = false;
		}

		private void zoomItem_Click(object sender, EventArgs e)
		{
			var ctrl = sender as ToolStripMenuItem;
			var percent = ctrl.Text.Remove(3);
			var value = int.Parse(percent);
			setZoom(value);
		}

		private void zoomTrackBar_ValueChanged(object sender, EventArgs e)
		{
			var zoom10 = Math.Pow(10, (((double)zoomTrackBar.Value / 150)+1));
			setZoom((int)(zoom10 * 10), trackBar: false);
		}

		private void editor_ZoomChanged(object sender, EventArgs e)
		{
			if (editor.Zoom < 25)
			{
				editor.Zoom = 25;
			}
			if (editor.Zoom > 450)
			{
				editor.Zoom = 450;
			}
			setZoom(editor.Zoom, editor: false);
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editor.Undo();
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editor.Redo();
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editor.Cut();
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editor.Copy();
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editor.Paste();
		}

		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editor.SelectAll();
		}

		private void findToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editor.ShowFindDialog();
		}

		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editor.ShowReplaceDialog();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		public string FileName
		{
			get { return fileName; }
			set
			{
				fileName = value;
				var title = fileName ?? "New file";
				this.Text = String.Format("{0}{1} - Assembler GUI", title, editor.IsChanged ? "*" : "");
			}
		}
		void updateTitle()
		{
			FileName = FileName;
		}

		private bool askForSave()
		{
			if (!editor.IsChanged)
			{
				return true;
			}
			var ans = MessageBox.Show(
				"Do you want to save changes to your program?",
				"Confirm close file",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question
			);
			if (ans == System.Windows.Forms.DialogResult.No)
			{
				return true;
			}
			if (ans == System.Windows.Forms.DialogResult.Yes)
			{
				saveToolStripMenuItem_Click(this, null);
				return !editor.IsChanged;
			}
			return false;
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (askForSave())
			{
				editor.IsChanged = false;
				FileName = null;
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (FileName == null)
			{
				saveAsToolStripMenuItem_Click(this, null);
			}
			else
			{
				editor.SaveToFile(FileName, Encoding.UTF8);
				updateTitle();
			}
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				var fileName = saveFileDialog.FileName;
				editor.SaveToFile(fileName, Encoding.UTF8);
				FileName = fileName;
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!askForSave())
			{
				e.Cancel = true;
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (askForSave())
			{
				if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					var fileName = openFileDialog.FileName;
					editor.OpenFile(fileName);
					FileName = fileName;
				}
			}
		}

		private void editor_UndoRedoStateChanged(object sender, EventArgs e)
		{
			undoToolStripButton.Enabled = editor.UndoEnabled;
			undoToolStripMenuItem.Enabled = editor.UndoEnabled;

			redoToolStripButton.Enabled = editor.RedoEnabled;
			redoToolStripMenuItem.Enabled = editor.RedoEnabled;
		}

		private void panelCloseBtn_MouseEnter(object sender, EventArgs e)
		{
			var label = sender as Label;
			label.BackColor = Color.CornflowerBlue;
		}

		private void panelCloseBtn_MouseLeave(object sender, EventArgs e)
		{
			var label = sender as Label;
			label.BackColor = Color.RoyalBlue;
		}

		private void errorPanel_VisibleChanged(object sender, EventArgs e)
		{
			errorsToolStripMenuItem.Checked = errorPanel.Visible;
			splitterErrors.Visible = errorPanel.Visible;
		}

		private void errorsPanelCloseBtn_Click(object sender, EventArgs e)
		{
			errorPanel.Hide();
		}

		private void errorsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			errorPanel.Visible = !errorPanel.Visible;
		}

		int opCodeSize = 0;
		private void opCodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (editor.LeftPadding < 1)
			{
				if (opCodeSize < 1)
				{
					opCodeSize = 90;
				}
				setOpCodeSize(opCodeSize);
			}
			else
			{
				opCodeSize = editor.LeftPadding;
				setOpCodeSize(0);
			}
		}

		private void errorsTabel_DoubleClick(object sender, EventArgs e)
		{
			if (errorsTable.SelectedRows.Count > 0)
			{
				var index = errorsTable.SelectedRows[0].Index;
				var error = compiler.Exceptions[index];
				var capture = error.CaptureInfo;

				Range range;
				if (capture.Length == 0)
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

				editor.Focus();
				editor.Selection = range;
				editor.DoSelectionVisible();
			}
		}

		private void errorsCountLabel_Click(object sender, EventArgs e)
		{
			errorPanel.Show();
		}

		private void symbolsCloseBtn_Click(object sender, EventArgs e)
		{
			symbolsPanel.Hide();
		}

		private void symbolsPanel_VisibleChanged(object sender, EventArgs e)
		{
			symbolsToolStripMenuItem.Checked = symbolsPanel.Visible;
			splitterSymbols.Visible = symbolsPanel.Visible;
		}

		private void symbolsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			symbolsPanel.Visible = !symbolsPanel.Visible;
		}

		private void variablesTable_DoubleClick(object sender, EventArgs e)
		{
			if (variablesTable.SelectedRows.Count > 0)
			{
				var index = variablesTable.SelectedRows[0].Index;
				var variable = compiler.Program.VariablesInfo[index];
				var line = variable.Line - 1;

				editor.Focus();
				editor.Selection = editor.GetLine(line);
				editor.DoSelectionVisible();
			}
		}

		private void labelsTable_DoubleClick(object sender, EventArgs e)
		{
			if (labelsTable.SelectedRows.Count > 0)
			{
				var index = labelsTable.SelectedRows[0].Index;
				var label = compiler.Program.LabelsInfo[index];
				var line = label.Line - 1;

				editor.Focus();
				editor.Selection = editor.GetLine(line);
				editor.DoSelectionVisible();
			}
		}

		private void proceduresTable_DoubleClick(object sender, EventArgs e)
		{
			if (proceduresTable.SelectedRows.Count > 0)
			{
				var index = proceduresTable.SelectedRows[0].Index;
				var procedure = compiler.Program.ProceduresInfo[index];
				var line = procedure.Line - 1;

				editor.Focus();
				editor.Selection = editor.GetLine(line);
				editor.DoSelectionVisible();
			}
		}

		private void collapseAllBlocksToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editor.CollapseAllFoldingBlocks();
		}

		private void expandAllBlocksToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editor.ExpandAllFoldingBlocks();
		}

		private void zoomTo100ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setZoom(100);
		}
	}
}
