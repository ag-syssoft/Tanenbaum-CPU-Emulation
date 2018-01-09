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
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.Error.WriteLine("Usage: trun filename");
				return;
			}
			int instructionCounter = 0;
			try
			{
				var inst = Machine.InstructionSequence.Parse(File.ReadAllLines(args[0]));
				var st = new Machine.State();


				while (true)
				{
					if (st.pc < 0 || st.pc >= inst.instructions.Count)
						break;

					instructionCounter++;
					var next = inst.instructions[st.pc];
					st.pc++;
					Console.WriteLine(next.line);
					if (!next.parameter.HasValue)
					{
						if (next.cmd != null)
							next.cmd(st, 0);
						else
							break;
					}
					else
						next.cmd(st, next.parameter.Value);

					foreach (string l in st.log)
						Console.WriteLine("    " + l);
					st.log.Clear();

				}
			}
			catch (Machine.CommandException ex)
			{
				Console.Error.WriteLine(ex.Message);
				Console.WriteLine("Known commands: ");
				Console.WriteLine("  [label]:");
				Console.WriteLine("  END");
				foreach (var cmd in Machine.Language.Commands)
					if (cmd.RequiresParameter)
					{
						if (cmd.WantsLabel)
							Console.WriteLine("  " + cmd.Name + " [label]");
						else
							Console.WriteLine("  " + cmd.Name + " [number]");
					}
					else
						Console.WriteLine("  " + cmd.Name);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
			}
			Console.WriteLine("Program ended after " + instructionCounter + " instruction(s)");

		}
	}
}
