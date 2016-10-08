namespace Assembler.GUI
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.editor = new FastColoredTextBoxNS.FastColoredTextBox();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.compileButton = new System.Windows.Forms.ToolStripButton();
			this.runButton = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.editor)).BeginInit();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// editor
			// 
			this.editor.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
			this.editor.AutoScrollMinSize = new System.Drawing.Size(357, 168);
			this.editor.BackBrush = null;
			this.editor.CharHeight = 14;
			this.editor.CharWidth = 8;
			this.editor.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.editor.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editor.IsReplaceMode = false;
			this.editor.LeftPadding = 90;
			this.editor.Location = new System.Drawing.Point(0, 25);
			this.editor.Name = "editor";
			this.editor.Paddings = new System.Windows.Forms.Padding(0);
			this.editor.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.editor.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("editor.ServiceColors")));
			this.editor.Size = new System.Drawing.Size(584, 447);
			this.editor.TabIndex = 4;
			this.editor.Text = "org 100h\r\nmov ah, 9\r\nlea dx, msg\r\nint 21h\r\n\r\nmov ax, 0x4c00 ; exit program\r\nint 2" +
    "1h\r\n\r\nmsg db \'Hello world!\'\r\n    db 10\r\n    db 13\r\n    db \'$\'";
			this.editor.Zoom = 100;
			this.editor.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.editor_TextChanged);
			this.editor.PaintLine += new System.EventHandler<FastColoredTextBoxNS.PaintLineEventArgs>(this.editor_PaintLine);
			// 
			// toolStrip
			// 
			this.toolStrip.BackColor = System.Drawing.Color.White;
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compileButton,
            this.runButton});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip.Size = new System.Drawing.Size(584, 25);
			this.toolStrip.TabIndex = 5;
			this.toolStrip.Text = "toolStrip";
			// 
			// compileButton
			// 
			this.compileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.compileButton.Image = ((System.Drawing.Image)(resources.GetObject("compileButton.Image")));
			this.compileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.compileButton.Name = "compileButton";
			this.compileButton.Size = new System.Drawing.Size(23, 22);
			this.compileButton.Text = "Compile";
			this.compileButton.Click += new System.EventHandler(this.compileButton_Click);
			// 
			// runButton
			// 
			this.runButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.runButton.Image = ((System.Drawing.Image)(resources.GetObject("runButton.Image")));
			this.runButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.runButton.Name = "runButton";
			this.runButton.Size = new System.Drawing.Size(23, 22);
			this.runButton.Text = "Run";
			this.runButton.Click += new System.EventHandler(this.runButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(584, 472);
			this.Controls.Add(this.editor);
			this.Controls.Add(this.toolStrip);
			this.Name = "MainForm";
			this.Text = "MainForm";
			((System.ComponentModel.ISupportInitialize)(this.editor)).EndInit();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private FastColoredTextBoxNS.FastColoredTextBox editor;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton compileButton;
		private System.Windows.Forms.ToolStripButton runButton;
	}
}