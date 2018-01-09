using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
	public class Execution
	{
		private Instruction[] program;
		private State state = new State();
		private int instructionCounter = 0;

		private Action<string> logOut;

		public Execution(Instruction[] program, Action<string> log)
		{
			this.program = program;
			if (log != null)
				logOut = log;
			else
				logOut = x => { };
		}

		public int InstructionCounter { get { return instructionCounter; } }
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
				if (inst.Action == null)
					return true;
				inst.Action(state);

				foreach (string l in state.log)
					logOut("    " + l);
				state.log.Clear();
			}
			return false;
		}


	}
}
