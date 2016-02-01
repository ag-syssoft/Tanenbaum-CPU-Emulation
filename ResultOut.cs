using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tanenbaum_CPU_Emulator
{
	public partial class ResultOut : Form
	{
		public ResultOut()
		{
			InitializeComponent();
		}

		internal void ClearLog()
		{
			resultBox.Items.Clear();
		}

		internal void Log(string line)
		{
			resultBox.Items.Add(line);
		}

		internal void LogFatal(string message)
		{
			resultBox.Items.Add("Fatal: "+message);
		}
	}
}
