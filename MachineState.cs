using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanenbaum_CPU_Emulator
{

	public class MachineState
	{
		public int ac = 0;  //accumulator
		public int pc = 0;  //program-counter
		public int sp = 0;  //stack-pointer
		public int[] m = new int[0x10000];

		public List<string> log = new List<string>();
		private void LogSP()
		{
			log.Add("sp -> " + (sp != 0 ? sp - m.Length : 0));
		}
		private void LogM(int a)
		{
			log.Add("m[" + a + "] -> " + m[a]);
		}
		private void LogPC()
		{
			log.Add("pc -> " + pc);
		}
		private void LogAC()
		{
			log.Add("ac -> " + ac);
		}


		public void Push()
		{
			sp--;
			if (sp < 0)
				sp = m.Length + sp;
			m[sp] = ac;
			LogSP();
			LogM(sp);
		}
		public void Pop()
		{
			ac = m[sp];
			sp++;
			if (sp >= m.Length)
				sp -= m.Length;
			LogAC();
			LogSP();
		}
		public void PushIndirect()
		{
			sp--;
			if (sp < 0)
				sp = m.Length + sp;
			m[sp] = m[ac];
			LogSP();
			LogM(sp);
		}
		public void PopIndirect()
		{
			m[ac] = m[sp];
			sp++;
			if (sp >= m.Length)
				sp -= m.Length;
			LogSP();
			LogM(ac);
		}

		public void JumpIfPositiveOrZero(int x)
		{
			if (ac >= 0)
			{
				pc = x;
				LogPC();
			}
		}
		public void JumpIfZero(int x)
		{
			if (ac == 0)
			{
				pc = x;
				LogPC();
			}
		}
		public void JumpIfNegative(int x)
		{
			if (ac < 0)
			{
				pc = x;
				LogPC();
			}
		}
		public void JumpIfNotZero(int x)
		{
			if (ac != 0)
			{
				pc = x;
				LogPC();
			}
		}
		public void Jump(int x)
		{
			pc = x;
			LogPC();
		}

		public void Call(int x)
		{
			sp--;
			if (sp < 0)
				sp = m.Length + sp;
			m[sp] = pc;
			pc = x;
			LogSP();
			LogM(sp);
			LogPC();
		}

		public void Return()
		{
			pc = m[sp];
			sp++;
			if (sp >= m.Length)
				sp -= m.Length;
			LogSP();
			LogPC();
		}

		public void Swap()
		{
			int tmp = ac;
			ac = sp;
			sp = tmp;
			LogAC();
			LogSP();
		}

		public void IncreaseStackPointer(int y)
		{
			sp = sp + y;
			if (sp < 0)
				sp = m.Length + sp;
			if (sp >= m.Length)
				sp -= m.Length;
			LogSP();
		}
		public void DecreaseStackPointer(int y)
		{
			sp = sp - y;
			if (sp < 0)
				sp = m.Length + sp;
			if (sp >= m.Length)
				sp -= m.Length;
			LogSP();
		}

		public void LoadDirect(int x)
		{
			CheckAddr(x);
			ac = m[x];
			LogAC();
		}
		public void StoreDirect(int x)
		{
			CheckAddr(x);
			m[x] = ac;
			LogM(x);
		}
		public void AddDirect(int x)
		{
			CheckAddr(x);
			ac = ac + m[x];
			LogAC();
		}
		public void SubDirect(int x)
		{
			CheckAddr(x);
			ac = ac - m[x];
			LogAC();
		}

		public void LoadStackRelative(int x)
		{
			CheckAddr(sp + x);
			ac = m[sp + x];
			LogAC();
		}
		public void StoreStackRelative(int x)
		{
			CheckAddr(sp + x);
			m[sp + x] = ac;
			LogM(sp + x);
		}
		public void AddStackRelative(int x)
		{
			CheckAddr(sp + x);
			ac = ac + m[sp + x];
			LogAC();
		}
		public void SubStackRelative(int x)
		{
			CheckAddr(sp + x);
			ac = ac - m[sp + x];
			LogAC();
		}
		public void LoadConstant(int x)
		{
			ac = x;
			LogAC();
		}

		private void CheckAddr(int x)
		{
			if (x < 0 || x >= m.Length)
			{
				throw new Exception("Index out of bounds for memory access: " + x + "/" + m.Length);

			}

		}

		public static void RegisterMethods()
		{
			CommandMap.RegisterWithParameter("LOCO", "LoadConstant");
			CommandMap.RegisterWithParameter("SUBL", "SubStackRelative");
			CommandMap.RegisterWithParameter("ADDL", "AddStackRelative");
			CommandMap.RegisterWithParameter("STOL", "StoreStackRelative");
			CommandMap.RegisterWithParameter("LODL", "LoadStackRelative");

			CommandMap.RegisterWithParameter("LODD", "LoadDirect");
			CommandMap.RegisterWithParameter("STOD", "StoreDirect");
			CommandMap.RegisterWithParameter("ADDD", "AddDirect");
			CommandMap.RegisterWithParameter("SUBD", "SubDirect");

			CommandMap.Register("PUSH", "Push");
			CommandMap.Register("POP", "Pop");
			CommandMap.Register("PSHI", "PushIndirect");
			CommandMap.Register("POPI", "PopIndirect");

			CommandMap.RegisterWithParameter("JPOS", "JumpIfPositiveOrZero");
			CommandMap.RegisterWithParameter("JZER", "JumpIfZero");
			CommandMap.RegisterWithParameter("JNEG", "JumpIfNegative");
			CommandMap.RegisterWithParameter("JNZE", "JumpIfNotZero");
			CommandMap.RegisterWithParameter("JUMP", "Jump");

			CommandMap.RegisterWithParameter("CALL", "Call");
			CommandMap.Register("RETN", "Return");
			CommandMap.Register("SWAP", "Swap");

			CommandMap.RegisterWithParameter("INSP", "IncreaseStackPointer");
			CommandMap.RegisterWithParameter("DESP", "DecreaseStackPointer");


			CommandMap.wantsLabel.Add("JPOS");
			CommandMap.wantsLabel.Add("JZER");
			CommandMap.wantsLabel.Add("JNEG");
			CommandMap.wantsLabel.Add("JNZE");
			CommandMap.wantsLabel.Add("JUMP");
			CommandMap.wantsLabel.Add("CALL");
		}
	}


}
