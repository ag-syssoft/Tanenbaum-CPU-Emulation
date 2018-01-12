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

			public enum ParameterType
			{
				None,
				Constant,
				StackAddress,
				StackDelta,
				Address,
				Label,
			}

			public readonly ParameterType Parameter;

			public bool RequiresParameter
			{
				get
				{
					return Parameter != ParameterType.None;
				}
			}



			public Command(string name, Action<State, int> action, ParameterType t)
			{
				Name = name;
				Action = action;
				if (t == ParameterType.None)
					throw new ArgumentException("Trying to declare instruction requiring a parameter, but type is set to none");
				Parameter = t;
			}
			public Command(string name, Action<State> action)
			{
				Name = name;
				Action = (state, param) => action(state);
				Parameter = ParameterType.None;
			}
		}


		private static Dictionary<string, Command> commands = new Dictionary<string, Command>();

		private static void Register(string name, Action<State, int> action, Command.ParameterType t)
		{
			commands.Add(name, new Command(name, action, t));
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
					throw new CommandDoesNotSupportParameterException(name);
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
			Register("LOCO", (s, p) => s.LoadConstant(p),Command.ParameterType.Constant);
			Register("SUBL", (s, p) => s.SubStackRelative(p), Command.ParameterType.StackAddress);
			Register("ADDL", (s, p) => s.AddStackRelative(p), Command.ParameterType.StackAddress);
			Register("STOL", (s, p) => s.StoreStackRelative(p), Command.ParameterType.StackAddress);
			Register("LODL", (s, p) => s.LoadStackRelative(p), Command.ParameterType.StackAddress);

			Register("LODD", (s, p) => s.LoadDirect(p), Command.ParameterType.Address);
			Register("STOD", (s, p) => s.StoreDirect(p), Command.ParameterType.Address);
			Register("ADDD", (s, p) => s.AddDirect(p), Command.ParameterType.Address);
			Register("SUBD", (s, p) => s.SubDirect(p), Command.ParameterType.Address);

			Register("PUSH", s => s.Push());
			Register("POP",  s => s.Pop());
			Register("PSHI", s => s.PushIndirect());
			Register("POPI", s => s.PopIndirect());

			Register("JPOS", (s, p) => s.JumpIfPositiveOrZero(p), Command.ParameterType.Label);
			Register("JZER", (s, p) => s.JumpIfZero(p), Command.ParameterType.Label);
			Register("JNEG", (s, p) => s.JumpIfNegative(p), Command.ParameterType.Label);
			Register("JNZE", (s, p) => s.JumpIfNotZero(p), Command.ParameterType.Label);
			Register("JUMP", (s, p) => s.Jump(p), Command.ParameterType.Label);

			Register("CALL", (s, p) => s.Call(p), Command.ParameterType.Label);
			Register("RETN", s => s.Return());
			Register("SWAP", s => s.Swap());

			Register("INSP", (s, p) => s.IncreaseStackPointer(p), Command.ParameterType.StackDelta);
			Register("DESP", (s, p) => s.DecreaseStackPointer(p), Command.ParameterType.StackDelta);

			Register("EXIT", s => s.Exit());
			Register("HALT", s => s.Halt());

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

			public override bool Equals(object o)
			{
				return o is ParsedSegment && ((ParsedSegment)o) == this;
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

			public static bool operator ==(ParsedSegment a, ParsedSegment b)
			{
				return a.Value == b.Value && a.Start == b.Start;
			}
			public static bool operator !=(ParsedSegment a, ParsedSegment b)
			{
				return !(a == b);
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

			public ParsedSegment[] Split(Func<char, bool> isSeparator, Func<char,bool> includeSeparators)
			{
				List<ParsedSegment> segs = new List<ParsedSegment>();
				int from = 0;
				for (int i = 0; i < Length; i++)
				{
					if (isSeparator(Value[i]))
					{
						if (from != i)
							segs.Add(Substring(from, i - from));
						if (includeSeparators != null && includeSeparators(Value[i]))
							segs.Add(Substring(i,1));
						from = i + 1;
					}
				}
				if (from != Length)
					segs.Add(Substring(from));
				return segs.ToArray();
			}

			public ParsedSegment[] SplitWhitespace()
			{
				return Split(Char.IsWhiteSpace, null);
			}

			public ParsedSegment ToUpper()
			{
				return new ParsedSegment(Value.ToUpper(), Start);
			}

			public override int GetHashCode()
			{
				var hashCode = -1754190471;
				hashCode = hashCode * -1521134295 + Value.GetHashCode();
				hashCode = hashCode * -1521134295 + Start.GetHashCode();
				return hashCode;
			}
		}

		public struct Alias
		{
			public readonly string Name;
			public readonly int Address;
			public readonly int? InitialValue;

			public Alias(string name, int address)
			{
				Name = name;
				Address = address;
				InitialValue = null;
			}

			public Alias(string name, int address, int? value)
			{
				Name = name;
				Address = address;
				InitialValue = value;
			}

			public override string ToString()
			{
				return "#alias " + Name + " @" + Address + (InitialValue.HasValue ?" ="+ InitialValue : "");
			}

		}

		public struct PreParsedLine
		{
			public readonly string InputLine;
			public readonly ParsedSegment Comment;
			public readonly ParsedSegment Label;
			public readonly ParsedSegment Command, Parameter, AliasName, AliasAddress, AliasValue;
			public readonly int IntAliasAddress;
			public readonly int? IntAliasValue;

			public Alias ToAlias()
			{
				return new Alias(AliasName.Value, IntAliasAddress, IntAliasValue);
			}

			public bool IsAlias
			{
				get
				{
					return !AliasName.IsEmpty;
				}
			}

			public bool IsCommand
			{
				get
				{
					return !Command.IsEmpty;
				}
			}

			public PreParsedLine(string input) : this()
			{
				InputLine = input;
				ParsedSegment source = new ParsedSegment(input);
				{
					int c0 = source.IndexOf("//");
					int c1 = source.IndexOf(";");
					if (c0 < 0)
						c0 = int.MaxValue;
					if (c1 < 0)
						c1 = int.MaxValue;
					int commentAt = Math.Min(c0, c1);



					if (commentAt < int.MaxValue)
					{
						Comment = new ParsedSegment(source, commentAt);
						source = source.Substring(0, commentAt);
					}
				}



				source = source.Trim();
				if (source.Length == 0)
					return;
				{
					int colon = source.IndexOf(":");
					if (colon >= 0)
					{
						Label = source.Substring(0, colon).Trim();
						source = source.Substring(colon + 1).Trim();
					}
				}

				if (source.Length == 0)
					return;
				{
					int hash = source.IndexOf("#");
					if (hash != -1)
					{
						//#alias <name> @<address> [=<value>]

						var elements = source.Substring(hash + 1).Split(c => c == '@' || c == '=' || Char.IsWhiteSpace(c), c => c == '@' || c == '=');

						if (elements.Length < 1)
							throw new ArgumentException("Expected precompiler instruction");

						if (elements[0] != "alias")
							throw new ArgumentException("#alias expected");
						if (elements.Length < 2)
							throw new ArgumentException("#alias <name> @<address> [=<value>]: Name expected");
						AliasName = elements[1];
						int x;
						if (int.TryParse(AliasName.Value,out x))
							throw new ArgumentException("#alias <name> @<address> [=<value>]: Name '"+AliasName.Value+"' is purely numeric, would be ignored by instructions");
						if (elements.Length < 3 || elements[2] != "@")
							throw new ArgumentException("#alias <name> @<address> [=<value>]: @ expected");
						if (elements.Length < 4)
							throw new ArgumentException("#alias <name> @<address> [=<value>]: Address expected");
						AliasAddress = elements[3];
						if (!int.TryParse(AliasAddress.Value, out IntAliasAddress))
							throw new ArgumentException("#alias <name> @<address> [=<value>]: Unable to parse address value '" + AliasAddress.Value + "'");
						if (IntAliasAddress < 0 || IntAliasAddress >= State.MemorySize)
							throw new ArgumentException("#alias <name> @<address> [=<value>]: Address value '" + IntAliasAddress + "' exceeds valid range");
						if (elements.Length > 4)
						{
							if (elements[4] != "=")
								throw new ArgumentException("#alias <name> @<address> [=<value>]: = expected");
							if (elements.Length < 6)
								throw new ArgumentException("#alias <name> @<address> [=<value>]: Value expected");
							AliasValue = elements[5];

							if (!int.TryParse(AliasValue.Value, out x))
								throw new ArgumentException("#alias <name> @<address> [=<value>]: Unable to parse value '" + AliasValue.Value + "'");
							IntAliasValue = x;
						}
						return;
					}
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

		public enum ParsedType
		{
			Constant,
			StackDelta,
			Address,
			Label,
			Alias,
		}

		private static int ParseNonNegative(PreParsedLine l)
		{
			int x = 0;
			if (!int.TryParse(l.Parameter.Value, out x))
				throw new ArgumentException("Unable to parse parameter '" + l.Parameter + "' of line '" + l.InputLine + "'");
			if (x < 0)
				throw new ArgumentException("Parameter '" + l.Parameter + "' of line '" + l.InputLine + "' must not be negative");
			if (x >= State.MemorySize)
				throw new ArgumentException("Parameter '" + l.Parameter + "' of line '" + l.InputLine + "' must be less than the address space size "+ State.MemorySize);

			return x;
		}

		public static int ParseParameter(Command.ParameterType t, PreParsedLine l,  out ParsedType foundType,  Dictionary<string,Alias> aliases, Dictionary<string, int> labels = null)
		{
			int x = 0;
			foundType = ParsedType.Constant;
			switch (t)
			{
				case Command.ParameterType.Label:
					if (labels != null && !labels.TryGetValue(l.Parameter.Value, out x))
						throw new ArgumentException("Unable to find label '" + l.Parameter + "' of line '" + l.InputLine + "'");
					foundType = ParsedType.Label;
					break;
				case Command.ParameterType.Constant:
					if (!int.TryParse(l.Parameter.Value, out x))
						throw new ArgumentException("Unable to parse parameter '" + l.Parameter + "' of line '" + l.InputLine + "'");
					break;
				case Command.ParameterType.StackDelta:
					x = ParseNonNegative(l);
					foundType = ParsedType.StackDelta;
					break;
				case Command.ParameterType.StackAddress:
					x = ParseNonNegative(l);
					foundType = ParsedType.Address;
					break;
				case Command.ParameterType.Address:
					foundType = ParsedType.Address;
					if (!int.TryParse(l.Parameter.Value, out x))
					{
						Alias alias;
						if (aliases.TryGetValue(l.Parameter.Value, out alias))
						{
							x = alias.Address;
							foundType = ParsedType.Alias;
						}
						else
							throw new ArgumentException("Unable to parse address parameter '" + l.Parameter + "' of line '" + l.InputLine + "'. Parameter is no valid integer, or known alias");
					}
					if (x < 0 || x >= State.MemorySize)
						throw new ArgumentException("Address parameter '" + l.Parameter + "' of line '" + l.InputLine + "' exceeds valid address range");
					break;
			}
			return x;
		}

		public static Instruction[] Parse(IEnumerable<string> lines)
		{
			int lineIndex = 0;
			var labels = new Dictionary<string, int>();
			var instructions = new List<Instruction>();
			lineIndex = 0;
			int commandCounter = 0;

			Dictionary<string, Alias> aliases = new Dictionary<string, Alias>();

			foreach (var line in lines)
			{
				lineIndex++;
				try
				{
					var l = new PreParsedLine(line);
					if (l.Label.HasValue)
					{
						if (labels.ContainsKey( l.Label.Value))
							throw new ArgumentException("Label '"+l.Label.Value+"' re-defined. Was originally defined at command #"+labels[l.Label.Value]);
						labels.Add(l.Label.Value, commandCounter);
					}
					if (l.IsAlias)
					{
						var alias = l.ToAlias();
						if (aliases.ContainsKey(alias.Name))
							throw new ArgumentException("Alias '" + alias.Name + "' already defined");
						aliases.Add(alias.Name, alias);

						if (l.IntAliasValue.HasValue)
							commandCounter ++;	//set
					}

					if (l.IsCommand)
						commandCounter++;
				}
				catch (Exception ex)
				{
					throw new CommandException(ex, line, lineIndex);
				}
			}

			foreach (var alias in aliases.Values)
			{
				if (alias.InitialValue.HasValue)
					instructions.Add(new Instruction(s => { s.m[alias.Address] = alias.InitialValue.Value; s.LogM(alias.Address); }, alias.ToString()));
			}

			lineIndex = 0;
			foreach (var line in lines)
			{
				lineIndex++;
				var l = new PreParsedLine(line);
				if (!l.IsCommand)
					continue;

				Action<State> action = null;
				string lineText = "";

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
					ParsedType ignore;
					int x = ParseParameter(command.Parameter, l, out ignore, aliases,labels);
					action = (s) => command.Action(s, x);
					lineText += command.Name + " " +
						"" + l.Parameter;
				}
				else
				{
					action = (s) => command.Action(s, 0);
					lineText += command.Name;
				}
				instructions.Add(new Instruction(action, lineText));
			}
			return instructions.ToArray();
		}

	}
}
