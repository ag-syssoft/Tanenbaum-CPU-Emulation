using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanenbaum_CPU_Emulator
{
	public class InstructionSequence
	{
		public struct Instruction
		{
			public Action<MachineState, int> pcmd;
			public Action<MachineState> cmd;
			public int parameter;
			public bool hasParameter;
			public string line, labelParameter;
		}

		public List<Instruction> instructions = new List<Instruction>();
		public Dictionary<string, int> labels = new Dictionary<string, int>();
	}
}
