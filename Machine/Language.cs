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


		public struct ParsedSegment
		{
			public readonly string Value;
			public readonly int Start;

			private ParsedSegment(string value, int start)
			{
				Value = value;
				Start = start;
			}

			public ParsedSegment(ParsedSegment source, int offset)
			{
				Start = offset + source.Start;
				Value = source.Value.Substring(offset);
			}
			public ParsedSegment(ParsedSegment source, int offset, int length)
			{
				Start = offset + source.Start;
				Value = source.Value.Substring(offset, length);
			}

			public ParsedSegment(string line)
			{
				Value = line;
				Start = 0;
			}

			public bool IsEmpty
			{
				get
				{
					return Value == null;
				}
			}
			public bool HasValue
			{
				get
				{
					return Value != null;
				}
			}

			public static bool operator ==(ParsedSegment seg, string str)
			{
				return seg.Value == str;
			}
			public static bool operator !=(ParsedSegment seg, string str)
			{
				return seg.Value != str;
			}

			public int IndexOf(string v)
			{
				return Value.IndexOf(v);
			}

			public ParsedSegment Substring(int start, int length)
			{
				return new ParsedSegment(this, start, length);
			}

			public override string ToString()
			{
				return Value ?? "<null>";
			}

			public int Length
			{
				get
				{
					return Value != null ? Value.Length : 0;
				}
			}

			public ParsedSegment Trim()
			{
				int start = 0;
				while (start < Value.Length && Char.IsWhiteSpace(Value[start]))
					start++;
				int end = Value.Length;
				while (end > 0 && Char.IsWhiteSpace(Value[end - 1]))
					end--;
				if (end <= start)
					return new ParsedSegment();
				return Substring(start, end - start);
			}

			public ParsedSegment Substring(int start)
			{
				return new ParsedSegment(this, start);
			}

			public ParsedSegment[] SplitWhitespace()
			{
				List<ParsedSegment> segs = new List<ParsedSegment>();
				int from = 0;
				for (int i = 0; i < Length; i++)
				{
					if (Char.IsWhiteSpace(Value[i]))
					{
						if (from != i)
							segs.Add(Substring(from, i - from));
						from = i + 1;
					}
				}
				if (from != Length)
					segs.Add(Substring(from));
				return segs.ToArray();
			}

			public ParsedSegment ToUpper()
			{
				return new ParsedSegment(Value.ToUpper(), Start);
			}
		}

		public struct PreParsedLine
		{
			public readonly ParsedSegment Comment;
			public readonly ParsedSegment Label;
			public readonly ParsedSegment Command, Parameter;

			public bool IsEmpty
			{
				get
				{
					return Command.IsEmpty;
				}
			}

			public PreParsedLine(ParsedSegment source) : this()
			{
				int at = source.IndexOf("//");
				if (at >= 0)
				{
					Comment = new ParsedSegment(source, at);
					source = source.Substring(0, at);
				}
				source = source.Trim();
				if (source.Length == 0)
					return;
				int colon = source.IndexOf(":");
				if (colon >= 0)
				{
					Label = source.Substring(0, colon).Trim();
					source = source.Substring(colon + 1).Trim();
				}
				var parts = source.SplitWhitespace();
				if (parts.Length > 0)
					Command = parts[0].ToUpper();
				if (parts.Length > 1)
					Parameter = parts[1];
				if (parts.Length > 2)
					throw new ArgumentException("Parsed line has more than two parts");
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
				try
				{
					var l = new PreParsedLine(new ParsedSegment(line));
					if (l.Label.HasValue)
						labels.Add(l.Label.Value, commandCounter);
					if (!l.IsEmpty)
						commandCounter++;
				}
				catch (Exception ex)
				{
					throw new CommandException(ex, line, lineIndex);
				}
			}


			lineIndex = 0;
			foreach (var line in lines)
			{
				lineIndex++;
				var l = new PreParsedLine(new ParsedSegment(line));
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
						command = Language.FindCommand(l.Command.Value, l.Parameter != null);
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
							if (!labels.TryGetValue(l.Parameter.Value, out x))
								throw new ArgumentException("Unable to find label '" + l.Parameter + "' of line '" + line + "'");
						}
						else if (!int.TryParse(l.Parameter.Value, out x))
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
