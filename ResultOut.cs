using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tanenbaum_CPU_Emulator
{
	public partial class ResultOut : Form
	{
		public ResultOut()
		{
			InitializeComponent();
		}

		internal void ClearLog()
		{
			resultBox.Items.Clear();
		}

		internal void Log(string line)
		{
			resultBox.Items.Add(line);
		}

		internal void LogFatal(string message)
		{
			resultBox.Items.Add("Fatal: "+message);
		}

		private Machine.Emulator exec;

		private void End()
		{
			if (exec != null)
			{
				Log("End");
				pauseButton.Enabled = false;
			}
			exec = null;
			programCounter.Enabled = false;
		}

		public void Run(Machine.Instruction[] program)
		{
			exec = new Machine.Emulator(program, x => Log(x));
			pauseButton.Text = "Pause";
			pauseButton.Enabled = true;
			programCounter.Enabled = true;
		}

		private void programCounter_Tick(object sender, EventArgs e)
		{
			if (exec == null)
			{
				End();
				return;
			}
			bool doEnd = false;
			try
			{
				if (exec.Execute(10))
					doEnd = true;
			}
			catch (Exception ex)
			{
				LogFatal(ex.Message);
				doEnd = true;
			}
			instructionCountLabel.Text = exec.InstructionCounter.ToString();
			pcLabel.Text = exec.State.pc.ToString();
			if (doEnd)
				End();
		}

		private void pauseButton_Click(object sender, EventArgs e)
		{
			if (exec != null)
			{
				programCounter.Enabled = !programCounter.Enabled;
				if (programCounter.Enabled)
					pauseButton.Text = "Pause";
				else
					pauseButton.Text = "Resume";
			}
		}
	}
}
