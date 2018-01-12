namespace Tanenbaum_CPU_Emulator
{
	partial class EditorForm
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.runToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.codeInputBox = new System.Windows.Forms.RichTextBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.menuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(801, 24);
			this.menuStrip1.TabIndex = 5;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// runToolStripMenuItem
			// 
			this.runToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem1});
			this.runToolStripMenuItem.Name = "runToolStripMenuItem";
			this.runToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.runToolStripMenuItem.Text = "Run";
			// 
			// runToolStripMenuItem1
			// 
			this.runToolStripMenuItem1.Name = "runToolStripMenuItem1";
			this.runToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.runToolStripMenuItem1.Size = new System.Drawing.Size(114, 22);
			this.runToolStripMenuItem1.Text = "Run";
			this.runToolStripMenuItem1.Click += new System.EventHandler(this.runToolStripMenuItem1_Click);
			// 
			// codeInputBox
			// 
			this.codeInputBox.AcceptsTab = true;
			this.codeInputBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.codeInputBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.codeInputBox.Location = new System.Drawing.Point(0, 24);
			this.codeInputBox.Name = "codeInputBox";
			this.codeInputBox.Size = new System.Drawing.Size(801, 567);
			this.codeInputBox.TabIndex = 6;
			this.codeInputBox.Text = "#alias one @1000 =1 ;declare \'one\' as new address alias, using address 1000 and i" +
    "nitial value 1\n\n//loop 10 times:\nloco 10\n\nloop:\n\tsubd one\n\tjnze loop\n\t\nexit";
			this.codeInputBox.TextChanged += new System.EventHandler(this.codeInputBox_TextChanged);
			this.codeInputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.codeInputBox_KeyDown);
			this.codeInputBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.codeInputBox_KeyPress);
			this.codeInputBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.codeInputBox_KeyUp);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 591);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(801, 22);
			this.statusStrip1.TabIndex = 7;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// statusLabel
			// 
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// EditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(801, 613);
			this.Controls.Add(this.codeInputBox);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "EditorForm";
			this.Text = "Tanenbaum CPU Emulator - Code";
			this.Shown += new System.EventHandler(this.EditorForm_Shown);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem1;
		private System.Windows.Forms.RichTextBox codeInputBox;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel statusLabel;
	}
}

