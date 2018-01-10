using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine
{
	/// <summary>
	/// Raw machine state, not including executed program
	/// </summary>
	public class State
	{
		public int ac = 0;  //accumulator
		public int pc = 0;  //program-counter
		public int sp = 0;  //stack-pointer
		public int[] m = new int[0x10000];

		public List<string> log = new List<string>();


		public State()
		{
			m[(int)Language.SpecialAddress.one] = 1;
		}

		private void Log(string msg)
		{
			log.Add(msg);
		}
		private void LogSP()
		{
			Log("sp := " + (sp != 0 ? sp - m.Length : 0));
		}
		private void LogM(int a)
		{
			Log("m[" + a + "] := " + m[a]);
		}
		private void LogPC()
		{
			Log("pc := " + pc);
		}
		private void LogAC()
		{
			Log("ac := " + ac);
		}



		public void Push()
		{
			Push(ac);
		}


		public void Pop()
		{
			Pop(ref ac);
			LogAC();
		}

		private void Pop(ref int popped)
		{
			popped = m[sp];
			sp++;
			if (sp >= m.Length)
				sp -= m.Length;
			LogSP();
		}

		private void Push(int x)
		{
			sp--;
			if (sp < 0)
				sp = m.Length + sp;
			m[sp] = x;
			LogSP();
			LogM(sp);
		}

		public void PushIndirect()
		{
			Push(m[ac]);
		}



		public void PopIndirect()
		{
			Pop(ref m[ac]);
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
			Push(pc);
			pc = x;
			LogPC();
		}


		public void Return()
		{
			Pop(ref pc);
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
			Log("ac += m[" + (x) + "]=" + m[x]);
			ac = ac + m[x];
			LogAC();
		}
		public void SubDirect(int x)
		{
			CheckAddr(x);
			Log("ac -= m[" + (x) + "]=" + m[x]);
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
			Log("ac += m[sp+" + (x) + "]=" + m[sp + x]);
			ac = ac + m[sp + x];
			LogAC();
		}
		public void SubStackRelative(int x)
		{
			CheckAddr(sp + x);
			Log("ac -= m[sp+" + (x) + "]=" + m[sp + x]);
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
				throw new ArgumentException("Index out of bounds for memory access: " + x + ". Should be in [0," + m.Length+")");
			}
		}

	}


}
