using System;
using System.Runtime.Serialization;

namespace Machine
{
	public class CommandException : Exception
	{
		public readonly string CommandName;
		public readonly string FullLine;
		public readonly int LineIndex;
		public CommandException(string commandName, string fullLine, int lineIndex)
		{
			CommandName = commandName;
			FullLine = fullLine;
			LineIndex = lineIndex;
		}


		public string Describe()
		{
			return "'"+CommandName + "' in line #" + LineIndex + ": '" + FullLine + "'";
		}
	}

	public class CommandNotFoundException : CommandException
	{

		public CommandNotFoundException(string commandName, string fullLine, int lineIndex) : base(commandName,fullLine,lineIndex)
		{}

		public override string Message
		{
			get
			{
				return "Unable to find command "+Describe();
			}
		}
	}

	public class CommandRequiresParameterException : CommandException
	{
		public CommandRequiresParameterException(string commandName, string fullLine, int lineIndex) : base(commandName, fullLine, lineIndex)
		{ }

		public override string Message
		{
			get
			{
				return "Command " + Describe()+" requires parameter";
			}
		}
	}



	public class CommandHasNoParameterException : CommandException
	{
		public CommandHasNoParameterException(string commandName, string fullLine, int lineIndex) : base(commandName, fullLine, lineIndex)
		{ }

		public override string Message
		{
			get
			{
				return "Command " + Describe() + " has no parameter, but parameter given";
			}
		}
	}

}
