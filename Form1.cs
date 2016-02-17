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


	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			MachineState.RegisterMethods();

		}

		private ResultOut resultOut;
		private void MakeLog()
		{
			if (resultOut == null)
				resultOut = new ResultOut();
			resultOut.Show();
			resultOut.Focus();
		}

		private void ClearLog()
		{
			MakeLog();
			resultOut.ClearLog();
		}

		private void Log(string line)
		{
			MakeLog();
			resultOut.Log(line);
		}

		private void LogFatal(string message)
		{
			MakeLog();
			resultOut.LogFatal(message);
		}

		private InstructionSequence ParseCode(string[] lines)
		{

			try
			{
				InstructionSequence p = new InstructionSequence();

				foreach (var line in lines)
				{
					string cmd = line;
					int at = cmd.IndexOf("//");
					if (at >= 0)
					{
						cmd = cmd.Substring(0, at);
					}
					cmd = cmd.Trim();
					if (cmd.Length == 0)
						continue;
					int colon = cmd.IndexOf(":");
					string label = null;
					if (colon >= 0)
					{
						label = cmd.Substring(0, colon).Trim();
						cmd = cmd.Substring(colon + 1).Trim();
					}
					InstructionSequence.Instruction inst = new InstructionSequence.Instruction();
					string[] parts = cmd.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < parts.Length; i++)
						parts[i] = parts[i].Trim();
					if (parts.Length == 0)
					{
						if (label != null)
							p.labels.Add(label, p.instructions.Count);
						continue;
					}
					if (label != null)
						inst.line = label + "(="+p.instructions.Count+"): ";
					else
						inst.line = "(" + p.instructions.Count + "): ";
					if (parts.Length == 2)
					{
						int x = 0;
						bool wantLabel = CommandMap.wantsLabel.Contains(parts[0]);
						if (wantLabel)
							inst.labelParameter = parts[1];
						if	(
								//(wantLabel && !p.labels.TryGetValue(parts[1], out x))
								//||
								(!wantLabel  && !int.TryParse(parts[1], out x))
							)
							{
								throw new Exception("Unable to parse parameter '" + parts[1] + "' of line '" + line + "'");
							}
						if (!CommandMap.parameterCommands.TryGetValue(parts[0], out inst.pcmd))
						{
							throw new Exception("Unable to find command '" + parts[0] + "' of line '" + line + "'");
						}
						inst.parameter = x;
						inst.hasParameter = true;
						inst.line += parts[0] + " " + (wantLabel ? parts[1] : x.ToString());
					}
					else
					{
						if (!CommandMap.plainCommands.TryGetValue(parts[0], out inst.cmd))
						{
							if (parts[0] == "END")
								inst.cmd = null;
							else
							{
								if (CommandMap.parameterCommands.ContainsKey(parts[0]))
									throw new Exception("Command '" + parts[0] + "' in line '" + line + "' requires a parameter");
								else
									throw new Exception("Unable to find command '" + parts[0] + "' of line '" + line + "'");
							}
						}
						inst.line += parts[0];
					}
					if (label != null)
					{
						p.labels.Add(label, p.instructions.Count);
					}
					p.instructions.Add(inst);
				}

				for (int i = 0; i < p.instructions.Count; i++)
				{
					if (p.instructions[i].labelParameter != null)
					{
						int x;
						InstructionSequence.Instruction inst = p.instructions[i];
						if (!p.labels.TryGetValue(inst.labelParameter, out x))
						{
							throw new Exception("Unable to find label '" + inst.labelParameter + "' of line '" + inst.line + "'");

						}
						inst.parameter = x;
						inst.line += " (=" + x+")";
						p.instructions[i] = inst;
					}

				}
				return p;
			}
			catch (Exception ex)
			{
				LogFatal(ex.ToString());
				return null;
			}
		}

		private void runToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			ClearLog();
			InstructionSequence p = ParseCode(codeInputBox.Lines);
			if (p == null)
				return;
			MakeLog();
			resultOut.Run(p);
		}
	}




}
