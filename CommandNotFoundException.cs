using System;
using System.Runtime.Serialization;

namespace Tanenbaum_CPU_Emulator
{
	public class CommandNotFoundException : Exception
	{
		public readonly string CommandName;
		public readonly string FullLine;
		public readonly int LineIndex;
		public readonly bool ExistsButRequiresParameter;

		public CommandNotFoundException(string commandName, string fullLine, int lineIndex, bool existsButRequiresParameter)
		{
			CommandName = commandName;
			FullLine = fullLine;
			LineIndex = lineIndex;
			ExistsButRequiresParameter = existsButRequiresParameter;
		}

		public override string Message
		{
			get
			{
				if (ExistsButRequiresParameter)
					return "Command '" + CommandName + "' in line #"+LineIndex+": '" + FullLine + "' requires a parameter";
				return "Unable to find command '" + CommandName + "' of line #"+LineIndex+": '" + FullLine + "'";
			}
		}

	}
}