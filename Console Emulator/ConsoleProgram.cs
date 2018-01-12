using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Emulator
{
	class ConsoleProgram
	{

		static void PrintState(Machine.Emulator exec)
		{
			Console.WriteLine("  Instructions executed: " + exec.InstructionCounter);
			Console.WriteLine("  SP=" + exec.State.sp + ", AC=" + exec.State.ac);
			Console.WriteLine("  Stack=" + exec.State.StackToString(8));
		}
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.Error.WriteLine("Usage: trun filename");
				return;
			}
			Machine.Emulator exec = null;
			try
			{
				exec = new Machine.Emulator(Machine.Language.Parse(File.ReadAllLines(args[0])), x => Console.WriteLine(x));

				bool end = false;

				while (!end)
				{
					switch (exec.Execute(int.MaxValue))
					{
						case Machine.Emulator.ExecutionState.Ended:
							end = true;
							break;
						case Machine.Emulator.ExecutionState.Paused:
							Console.WriteLine("Program paused");
							PrintState(exec);
							Console.WriteLine("  (Press enter to continue)");
							Console.ReadLine();
							break;
					}
				}
			}
			catch (Machine.CommandException ex)
			{
				Console.Error.WriteLine(ex.Message);
				if (ex.InnerException is Machine.CommandNotFoundException)
				{
					Console.WriteLine("Known commands: ");
					Console.WriteLine("  [label]:");
					foreach (var cmd in Machine.Language.Commands)
						if (cmd.RequiresParameter)
							Console.WriteLine("  " + cmd.Name + " [" + cmd.Parameter + "]");
						else
							Console.WriteLine("  " + cmd.Name);
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
			}
			if (exec != null)
			{
				Console.WriteLine("Program exited");
				PrintState(exec);
			}

		}
	}
}
