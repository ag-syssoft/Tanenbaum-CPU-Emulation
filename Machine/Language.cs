using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
	public static class Language
	{
		public struct Command
		{
			public readonly string Name;
			public readonly Action<State, int> Action;
			public readonly bool RequiresParameter, WantsLabel;


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


		static Dictionary<string, Command> commands = new Dictionary<string, Command>();

		public static void Register(string name, Action<State, int> action, bool wantsLabel = false)
		{
			commands.Add(name, new Command(name, action, wantsLabel));
		}
		public static void Register(string name, Action<State> action)
		{
			commands.Add(name, new Command(name, action));
		}

		public static Command FindCommand(string name, string line, int lineIndex, bool haveParameter)
		{
			Command rs;
			name = name.ToUpper();
			if (!commands.TryGetValue(name, out rs))
				throw new CommandNotFoundException(name, line, lineIndex);
			if (rs.RequiresParameter != haveParameter)
			{
				if (rs.RequiresParameter)
					throw new CommandRequiresParameterException(name, line, lineIndex);
				else
					throw new CommandHasNoParameterException(name, line, lineIndex);
			}
			return rs;
		}

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

			Register("PUSH", (s) => s.Push());
			Register("POP", (s) => s.Pop());
			Register("PSHI", (s) => s.PushIndirect());
			Register("POPI", (s) => s.PopIndirect());

			Register("JPOS", (s, p) => s.JumpIfPositiveOrZero(p),true);
			Register("JZER", (s, p) => s.JumpIfZero(p), true);
			Register("JNEG", (s, p) => s.JumpIfNegative(p),true);
			Register("JNZE", (s, p) => s.JumpIfNotZero(p), true);
			Register("JUMP", (s, p) => s.Jump(p), true);

			Register("CALL", (s, p) => s.Call(p), true);
			Register("RETN", (s) => s.Return());
			Register("SWAP", (s) => s.Swap());

			Register("INSP", (s, p) => s.IncreaseStackPointer(p));
			Register("DESP", (s, p) => s.DecreaseStackPointer(p));
		}
	}
}
