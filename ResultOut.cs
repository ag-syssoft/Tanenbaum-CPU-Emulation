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


		private Machine.State machineState;
		private Machine.InstructionSequence sequence;
		private bool canResume = true;
		private int instructionCounter = 0;

		private void End()
		{
			if (canResume)
			{
				Log("End");
				pauseButton.Enabled = false;
			}
			canResume = false;
			programCounter.Enabled = false;
		}

		public void Run(Machine.InstructionSequence seq)
		{
			machineState = new Machine.State();
			sequence = seq;
			pauseButton.Text = "Pause";
			pauseButton.Enabled = true;
			canResume = true;
			programCounter.Enabled = true;
			instructionCounter = 0;
		}

		private void programCounter_Tick(object sender, EventArgs e)
		{
			Machine.State st = machineState;
			Machine.InstructionSequence p = sequence;
			if (!canResume)
			{
				End();
				return;
			}
			try
			{
				for (int i = 0; i < 10; i++)
				{
					if (st.pc < 0 || st.pc >= p.instructions.Count)
					{
						End();
						break;
					}
					instructionCounter++;
					Machine.InstructionSequence.Instruction inst = p.instructions[st.pc];
					st.pc++;
					Log(inst.line);
					if (!inst.parameter.HasValue)
					{
						if (inst.cmd != null)
							inst.cmd(st,0);
						else
						{
							End();
							break;
						}
					}
					else
						inst.cmd(st, inst.parameter.Value);

					foreach (string l in st.log)
						Log("    " + l);
					st.log.Clear();
				}
			}
			catch (Exception ex)
			{
				LogFatal(ex.Message);
				End();
			}
			instructionCountLabel.Text = instructionCounter.ToString();
			pcLabel.Text = st.pc.ToString();

		}

		private void pauseButton_Click(object sender, EventArgs e)
		{
			if (canResume)
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
