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


	public partial class EditorForm : Form
	{
		public EditorForm()
		{
			InitializeComponent();

			MachineState.RegisterMethods();

		}

		private ResultOut resultOut;
		private void MakeLog()
		{
			if (resultOut == null || !resultOut.Visible)
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
				int lineIndex = 0;
				foreach (var line in lines)
				{
					lineIndex++;
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
					string commandName = parts[0].ToUpper();
					if (label != null)
						inst.line = label + "(=" + p.instructions.Count + "): ";
					else
						inst.line = "(" + p.instructions.Count + "): ";
					if (parts.Length == 2)
					{
						int x = 0;
						bool wantLabel = CommandMap.wantsLabel.Contains(commandName);
						if (wantLabel)
							inst.labelParameter = parts[1];
						if (
								//(wantLabel && !p.labels.TryGetValue(parts[1], out x))
								//||
								(!wantLabel && !int.TryParse(parts[1], out x))
							)
						{
							throw new Exception("Unable to parse parameter '" + parts[1] + "' of line '" + line + "'");
						}
						if (!CommandMap.parameterCommands.TryGetValue(commandName, out inst.pcmd))
							throw new CommandNotFoundException(commandName, line, lineIndex, false);
						inst.parameter = x;
						inst.hasParameter = true;
						inst.line += commandName + " " + (wantLabel ? parts[1] : x.ToString());
					}
					else
					{
						if (!CommandMap.plainCommands.TryGetValue(commandName, out inst.cmd))
						{
							if (parts[0] == "END")
								inst.cmd = null;
							else
							{
								throw new CommandNotFoundException(commandName, line, lineIndex, CommandMap.parameterCommands.ContainsKey(commandName));
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
						inst.line += " (=" + x + ")";
						p.instructions[i] = inst;
					}

				}
				return p;
			}
			catch (CommandNotFoundException ex)
			{
				LogFatal(ex.Message);
				Log("Known commands: ");
				Log("  [label]:");
				foreach (var cmd in CommandMap.plainCommands)
					Log("  " + cmd.Key);
				foreach (var cmd in CommandMap.parameterCommands)
					if (CommandMap.wantsLabel.Contains(cmd.Key))
						Log("  "+cmd.Key + " [label]");
					else
						Log("  " + cmd.Key + " [number]");
			}
			catch (Exception ex)
			{
				LogFatal(ex.Message);
			}
			return null;
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
