using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
	/// <summary>
	/// Machine execution state.
	/// This is the central class to create when running the Tanenbaum emulator
	/// </summary>
	public class Emulator
	{
		private Instruction[] program;
		private State state = new State();
		private int instructionCounter = 0;

		private Action<string> logOut;

		/// <summary>
		/// Constructs a new execution from a given program
		/// </summary>
		/// <param name="program">Program to execute. May be empty or null, causing the execution to immediately end</param>
		/// <param name="log">Logging function. May be null, disabling log output</param>
		public Emulator(Instruction[] program, Action<string> log)
		{
			this.program = program ?? new Instruction[0];
			if (log != null)
				logOut = log;
			else
				logOut = x => { };
		}

		/// <summary>
		/// Number of instructions that have been executed
		/// </summary>
		public int InstructionCounter { get { return instructionCounter; } }
		/// <summary>
		/// Internal machine state
		/// </summary>
		public State State { get { return state; } }

		/// <summary>
		/// Executes a number of instructions on the local state
		/// </summary>
		/// <param name="maxInstructions">Maximum number of instructions to execute</param>
		/// <returns>False if the program has not terminated, true if it has reached some program end state in the alotted number of instructions</returns>
		public bool Execute(int maxInstructions)
		{
			for (int i = 0; i < maxInstructions; i++)
			{
				if (state.pc < 0 || state.pc >= program.Length)
					return true;
				instructionCounter++;
				var inst = program[state.pc];
				state.pc++;
				logOut(inst.Caption);
				inst.Action(state);

				foreach (string l in state.log)
					logOut("    " + l);
				state.log.Clear();
			}
			return false;
		}


	}
}
