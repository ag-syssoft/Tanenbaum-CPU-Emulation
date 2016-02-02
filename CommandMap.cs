using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanenbaum_CPU_Emulator
{
	public static class CommandMap
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


}
