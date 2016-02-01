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
						bool wantLabel = Command.wantsLabel.Contains(parts[0]);
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
						if (!Command.parameterCommands.TryGetValue(parts[0], out inst.pcmd))
						{
							throw new Exception("Unable to find command '" + parts[0] + "' of line '" + line + "'");
						}
						inst.parameter = x;
						inst.hasParameter = true;
						inst.line += parts[0] + " " + (wantLabel ? parts[1] : x.ToString());
					}
					else
					{
						if (!Command.plainCommands.TryGetValue(parts[0], out inst.cmd))
						{
							if (parts[0] == "END")
								inst.cmd = null;
							else
								throw new Exception("Unable to find parameter-less command '" + parts[0] + "' of line '" + line + "'");
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
			MachineState st = new MachineState();
			try
			{
				while (true)
				{
					if (st.pc < 0 || st.pc >= p.instructions.Count)
						break;
					InstructionSequence.Instruction inst = p.instructions[st.pc];
					st.pc++;
					Log(inst.line);
					if (!inst.hasParameter)
					{
						if (inst.cmd != null)
							inst.cmd(st);
						else
							break;
					}
					else
						inst.pcmd(st, inst.parameter);

					foreach (string l in st.log)
						Log("    " + l);
					st.log.Clear();
				}
				Log("End");
			}
			catch (Exception ex)
			{
				LogFatal(ex.ToString());
			}

		}
	}

	public class MachineState
	{
		public int ac = 0;  //accumulator
		public int pc = 0;  //program-counter
		public int sp = 0;  //stack-pointer
		public int[] m = new int[0x10000];

		public List<string> log = new List<string>();
		private void LogSP()
		{
			log.Add("sp -> " + (sp != 0 ? sp-m.Length : 0));
		}
		private void LogM(int a)
		{
			log.Add("m[" + a + "] -> " + m[a]);
		}
		private void LogPC()
		{
			log.Add("pc -> " + pc);
		}
		private void LogAC()
		{
			log.Add("ac -> " + ac);
		}


		public void Push()
		{
			sp--;
			if (sp < 0)
				sp = m.Length + sp;
			m[sp] = ac;
			LogSP();
			LogM(sp);
		}
		public void Pop()
		{
			ac = m[sp];
			sp++;
			if (sp >= m.Length)
				sp -= m.Length;
			LogAC();
			LogSP();
		}
		public void PushIndirect()
		{
			sp--;
			if (sp < 0)
				sp = m.Length + sp;
			m[sp] = m[ac];
			LogSP();
			LogM(sp);
		}
		public void PopIndirect()
		{
			m[ac] = m[sp];
			sp++;
			if (sp >= m.Length)
				sp -= m.Length;
			LogSP();
			LogM(ac);
		}

		public void JumpIfPositiveOrZero(int x)
		{
			if (ac >= 0)
			{
				pc = x;
				LogPC();
			}
		}
		public void JumpIfZero(int x)
		{
			if (ac == 0)
			{
				pc = x;
				LogPC();
			}
		}
		public void JumpIfNegative(int x)
		{
			if (ac < 0)
			{
				pc = x;
				LogPC();
			}
		}
		public void JumpIfNotZero(int x)
		{
			if (ac != 0)
			{
				pc = x;
				LogPC();
			}
		}
		public void Jump(int x)
		{
			pc = x;
			LogPC();
		}

		public void Call(int x)
		{
			sp--;
			if (sp < 0)
				sp = m.Length + sp;
			m[sp] = pc;
			pc = x;
			LogSP();
			LogM(sp);
			LogPC();
		}

		public void Return()
		{
			pc = m[sp];
			sp++;
			if (sp >= m.Length)
				sp -= m.Length;
			LogSP();
			LogPC();
		}

		public void Swap()
		{
			int tmp = ac;
			ac = sp;
			sp = tmp;
			LogAC();
			LogSP();
		}

		public void IncreaseStackPointer(int y)
		{
			sp = sp + y;
			if (sp < 0)
				sp = m.Length + sp;
			if (sp >= m.Length)
				sp -= m.Length;
			LogSP();
		}
		public void DecreaseStackPointer(int y)
		{
			sp = sp - y;
			if (sp < 0)
				sp = m.Length + sp;
			if (sp >= m.Length)
				sp -= m.Length;
			LogSP();
		}

		public void LoadDirect(int x)
		{
			CheckAddr(x);
			ac = m[x];
			LogAC();
		}
		public void StoreDirect(int x)
		{
			CheckAddr(x);
			m[x] = ac;
			LogM(x);
		}
		public void AddDirect(int x)
		{
			CheckAddr(x);
			ac = ac + m[x];
			LogAC();
		}
		public void SubDirect(int x)
		{
			CheckAddr(x);
			ac = ac - m[x];
			LogAC();
		}

		public void LoadStackRelative(int x)
		{
			CheckAddr(sp + x);
			ac = m[sp + x];
			LogAC();
		}
		public void StoreStackRelative(int x)
		{
			CheckAddr(sp + x);
			m[sp + x] = ac;
			LogM(sp + x);
		}
		public void AddStackRelative(int x)
		{
			CheckAddr(sp + x);
			ac = ac + m[sp + x];
			LogAC();
		}
		public void SubStackRelative(int x)
		{
			CheckAddr(sp + x);
			ac = ac - m[sp + x];
			LogAC();
		}
		public void LoadConstant(int x)
		{
			ac = x;
			LogAC();
		}

		private void CheckAddr(int x)
		{
			if (x < 0 || x >= m.Length)
			{
				throw new Exception("Index out of bounds for memory access: " + x + "/" + m.Length);

			}

		}

		public static void RegisterMethods()
		{
			Command.RegisterWithParameter("LOCO", "LoadConstant");
			Command.RegisterWithParameter("SUBL", "SubStackRelative");
			Command.RegisterWithParameter("ADDL", "AddStackRelative");
			Command.RegisterWithParameter("STOL", "StoreStackRelative");
			Command.RegisterWithParameter("LODL", "LoadStackRelative");

			Command.RegisterWithParameter("LODD", "LoadDirect");
			Command.RegisterWithParameter("STOD", "StoreDirect");
			Command.RegisterWithParameter("ADDD", "AddDirect");
			Command.RegisterWithParameter("SUBD", "SubDirect");

			Command.Register("PUSH", "Push");
			Command.Register("POP", "Pop");
			Command.Register("PSHI", "PushIndirect");
			Command.Register("POPI", "PopIndirect");

			Command.RegisterWithParameter("JPOS", "JumpIfPositiveOrZero");
			Command.RegisterWithParameter("JZER", "JumpIfZero");
			Command.RegisterWithParameter("JNEG", "JumpIfNegative");
			Command.RegisterWithParameter("JNZE", "JumpIfNotZero");
			Command.RegisterWithParameter("JUMP", "Jump");

			Command.RegisterWithParameter("CALL", "Call");
			Command.Register("RETN", "Return");
			Command.Register("SWAP", "Swap");

			Command.RegisterWithParameter("INSP", "IncreaseStackPointer");
			Command.RegisterWithParameter("DESP", "DecreaseStackPointer");


			Command.wantsLabel.Add("JPOS");
			Command.wantsLabel.Add("JZER");
			Command.wantsLabel.Add("JNEG");
			Command.wantsLabel.Add("JNZE");
			Command.wantsLabel.Add("JUMP");
			Command.wantsLabel.Add("CALL");
		}
	}



	public static class Command
	{
		public static Dictionary<string, Action<MachineState, int>> parameterCommands = new Dictionary<string, Action<MachineState, int>>();
		public static Dictionary<string, Action<MachineState>> plainCommands = new Dictionary<string, Action<MachineState>>();

		public static HashSet<string> wantsLabel = new HashSet<string>();


		public static void RegisterWithParameter(string name, string methodName)
		{
			Action<MachineState, int> cmd = (Action<MachineState, int>)
				Delegate.CreateDelegate(typeof(Action<MachineState, int>), typeof(MachineState).GetMethod(methodName));
			parameterCommands.Add(name, cmd);
		}
		public static void Register(string name, string methodName)
		{
			Action<MachineState> cmd = (Action<MachineState>)
				Delegate.CreateDelegate(typeof(Action<MachineState>), typeof(MachineState).GetMethod(methodName));
			plainCommands.Add(name, cmd);
		}
	}




	public class InstructionSequence
	{
		public struct Instruction
		{
			public Action<MachineState, int> pcmd;
			public Action<MachineState> cmd;
			public int parameter;
			public bool hasParameter;
			public string line,labelParameter;
		}

		public List<Instruction> instructions = new List<Instruction>();
		public Dictionary<string, int> labels = new Dictionary<string, int>();
	}

}
