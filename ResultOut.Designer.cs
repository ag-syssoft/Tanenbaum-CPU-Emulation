namespace Tanenbaum_CPU_Emulator
{
	partial class ResultOut
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
			this.resultBox = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// resultBox
			// 
			this.resultBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.resultBox.FormattingEnabled = true;
			this.resultBox.Location = new System.Drawing.Point(0, 0);
			this.resultBox.Name = "resultBox";
			this.resultBox.Size = new System.Drawing.Size(547, 474);
			this.resultBox.TabIndex = 4;
			// 
			// ResultOut
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(547, 474);
			this.Controls.Add(this.resultBox);
			this.Name = "ResultOut";
			this.Text = "ResultOut";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox resultBox;
	}
}