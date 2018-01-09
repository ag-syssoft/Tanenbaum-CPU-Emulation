using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
	public class InstructionSequence
	{
		public struct Instruction
		{
			public Action<State, int> cmd;
			public int? parameter;
			public string line, labelParameter;
		}

		public List<Instruction> instructions = new List<Instruction>();
		public Dictionary<string, int> labels = new Dictionary<string, int>();


		public static InstructionSequence Parse(IEnumerable<string> lines)
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
				Instruction inst = new Instruction();
				string[] parts = cmd.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < parts.Length; i++)
					parts[i] = parts[i].Trim();
				if (parts.Length == 0)
				{
					if (label != null)
						p.labels.Add(label, p.instructions.Count);
					continue;
				}
				if (parts[0].ToUpper() == "END")
					inst.cmd = null;
				else
				{
					Language.Command command = Language.FindCommand(parts[0],line,lineIndex,parts.Length==2);
					if (label != null)
						inst.line = label + "(=" + p.instructions.Count + "): ";
					else
						inst.line = "(" + p.instructions.Count + "): ";



					if (command.RequiresParameter)
					{
						int x = 0;
						bool wantLabel = command.WantsLabel;
						if (wantLabel)
							inst.labelParameter = parts[1];
						if (
								//(wantLabel && !p.labels.TryGetValue(parts[1], out x))
								//||
								(!wantLabel && !int.TryParse(parts[1], out x))
							)
						{
							throw new ArgumentException("Unable to parse parameter '" + parts[1] + "' of line '" + line + "'");
						}
						inst.parameter = x;
						inst.line += command.Name + " " + parts[1];
					}
					else
					{
						inst.line += parts[0];
					}
					inst.cmd = command.Action;
					if (label != null)
					{
						p.labels.Add(label, p.instructions.Count);
					}
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

	}
}
