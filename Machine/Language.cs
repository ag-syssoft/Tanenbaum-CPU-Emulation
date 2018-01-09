using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
	/// <summary>
	/// Compiled program instruction
	/// </summary>
	public struct Instruction
	{
		/// <summary>
		/// Action to execute. Includes any potential parameters
		/// </summary>
		public readonly Action<State> Action;
		/// <summary>
		/// Caption to log upon execution
		/// </summary>
		public readonly string Caption;

		public Instruction(Action<State> cmd, string line)
		{
			this.Action = cmd;
			this.Caption = line;
		}
	}

	/// <summary>
	/// Declarations of supported commands. Self-constructs
	/// </summary>
	public static class Language
	{
		/// <summary>
		/// Declared command supported by the local language
		/// </summary>
		public struct Command
		{
			/// <summary>
			/// Command name (always upper case)
			/// </summary>
			public readonly string Name;
			/// <summary>
			/// Action to execute when this command is triggered.
			/// Requires the state to modify, and the parameter specified during compilation
			/// </summary>
			public readonly Action<State, int> Action;
			/// <summary>
			/// True if the local command requires a parameter
			/// </summary>
			public readonly bool RequiresParameter;
			/// <summary>
			/// True if the local command requires a parameter which should be interpreted as a label.
			/// False if RequiresParameter is false.
			/// </summary>
			public readonly bool WantsLabel;

			public Command(string name, Action<State, int> action, bool wantsLabel)
			{
				Name = name;
				Action = action;
				WantsLabel = wantsLabel;
				RequiresParameter = true;
			}
			public Command(string name, Action<State> action)
			{
				Name = name;
				Action = (state, param) => action(state);
				WantsLabel = false;
				RequiresParameter = false;
			}
		}


		private static Dictionary<string, Command> commands = new Dictionary<string, Command>();

		private static void Register(string name, Action<State, int> action, bool wantsLabel = false)
		{
			commands.Add(name, new Command(name, action, wantsLabel));
		}
		private static void Register(string name, Action<State> action)
		{
			commands.Add(name, new Command(name, action));
		}

		/// <summary>
		/// Searches for a command matching the specified parameters.
		/// Throws exceptions if the command is not found or does not match the expectations.
		/// </summary>
		/// <param name="name">Name of the command to find. Case is ignored.</param>
		/// <param name="haveParameter">Set true if a parameter is specified by the program being compiled. The found command will be tested for compliance.</param>
		/// <returns>Found command</returns>
		public static Command FindCommand(string name, bool haveParameter)
		{
			Command rs;
			name = name.ToUpper();
			if (!commands.TryGetValue(name, out rs))
				throw new CommandNotFoundException(name);
			if (rs.RequiresParameter != haveParameter)
			{
				if (rs.RequiresParameter)
					throw new CommandRequiresParameterException(name);
				else
					throw new CommandHasNoParameterException(name);
			}
			return rs;
		}

		/// <summary>
		/// Queries an enumerable list of all known commands
		/// </summary>
		public static IEnumerable<Command> Commands
		{
			get
			{
				return commands.Values;
			}
		}


		static Language()
		{
			Register("LOCO", (s, p) => s.LoadConstant(p));
			Register("SUBL", (s, p) => s.SubStackRelative(p));
			Register("ADDL", (s, p) => s.AddStackRelative(p));
			Register("STOL", (s, p) => s.StoreStackRelative(p));
			Register("LODL", (s, p) => s.LoadStackRelative(p));

			Register("LODD", (s, p) => s.LoadDirect(p));
			Register("STOD", (s, p) => s.StoreDirect(p));
			Register("ADDD", (s, p) => s.AddDirect(p));
			Register("SUBD", (s, p) => s.SubDirect(p));

			Register("PUSH", s => s.Push());
			Register("POP",  s => s.Pop());
			Register("PSHI", s => s.PushIndirect());
			Register("POPI", s => s.PopIndirect());

			Register("JPOS", (s, p) => s.JumpIfPositiveOrZero(p),true);
			Register("JZER", (s, p) => s.JumpIfZero(p), true);
			Register("JNEG", (s, p) => s.JumpIfNegative(p),true);
			Register("JNZE", (s, p) => s.JumpIfNotZero(p), true);
			Register("JUMP", (s, p) => s.Jump(p), true);

			Register("CALL", (s, p) => s.Call(p), true);
			Register("RETN", s => s.Return());
			Register("SWAP", s => s.Swap());

			Register("INSP", (s, p) => s.IncreaseStackPointer(p));
			Register("DESP", (s, p) => s.DecreaseStackPointer(p));
		}



		private struct Line
		{
			public readonly string Comment;
			public readonly string Label;
			public readonly string Command, Parameter;

			public bool IsEmpty
			{
				get
				{
					return Command == null;
				}
			}

			public Line(string line) : this()
			{
				string cmd = line;
				int at = cmd.IndexOf("//");
				if (at >= 0)
				{
					Comment = cmd.Substring(at + 2);
					cmd = cmd.Substring(0, at);
				}
				cmd = cmd.Trim();
				if (cmd.Length == 0)
					return;
				int colon = cmd.IndexOf(":");
				if (colon >= 0)
				{
					Label = cmd.Substring(0, colon).Trim();
					cmd = cmd.Substring(colon + 1).Trim();
				}
				var parts = cmd.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < parts.Length; i++)
					parts[i] = parts[i].Trim();
				if (parts.Length > 0)
					Command = parts[0].ToUpper();
				if (parts.Length > 1)
					Parameter = parts[1];
			}

		}

		public static Instruction[] Parse(IEnumerable<string> lines)
		{
			int lineIndex = 0;
			var labels = new Dictionary<string, int>();
			var instructions = new List<Instruction>();
			lineIndex = 0;
			int commandCounter = 0;
			foreach (var line in lines)
			{
				lineIndex++;
				var l = new Line(line);
				if (l.Label != null)
					labels.Add(l.Label, commandCounter);
				if (!l.IsEmpty)
					commandCounter++;
			}


			lineIndex = 0;
			foreach (var line in lines)
			{
				lineIndex++;
				var l = new Line(line);
				if (l.IsEmpty)
					continue;

				Action<State> action = null;
				string lineText = "";

				if (l.Command == "END")
				{ }
				else
				{

					Language.Command command;
					try
					{
						command = Language.FindCommand(l.Command, l.Parameter != null);
					}
					catch (Exception ex)
					{
						throw new CommandException(ex, line, lineIndex);
					}
					if (l.Label != null)
						lineText = l.Label + "(=" + instructions.Count + "): ";
					else
						lineText = "(" + instructions.Count + "): ";



					if (command.RequiresParameter)
					{
						int x = 0;
						bool wantLabel = command.WantsLabel;
						if (wantLabel)
						{
							if (!labels.TryGetValue(l.Parameter, out x))
								throw new ArgumentException("Unable to find label '" + l.Parameter + "' of line '" + line + "'");
						}
						else if (!int.TryParse(l.Parameter, out x))
						{
							throw new ArgumentException("Unable to parse parameter '" + l.Parameter + "' of line '" + line + "'");
						}
						action = (s) => command.Action(s, x);
						lineText += command.Name + " " + l.Parameter;
					}
					else
					{
						action = (s) => command.Action(s, 0);
						lineText += command.Name;
					}
				}
				instructions.Add(new Instruction(action, lineText));
			}

			return instructions.ToArray();
		}

	}
}
