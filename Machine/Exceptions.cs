using System;
using System.Runtime.Serialization;

namespace Machine
{
	public class CommandException : Exception
	{
		public readonly string FullLine;
		public readonly int LineIndex;
		public CommandException(Exception nested, string fullLine, int lineIndex): base("Line #"+lineIndex+" '"+fullLine+"': "+nested.Message,nested)
		{
			FullLine = fullLine;
			LineIndex = lineIndex;
		}
	}

	public class CommandNotFoundException : Exception
	{

		public CommandNotFoundException(string commandName) : base("Unable to find command '"+commandName+"'")
		{}

	}

	public class CommandRequiresParameterException : Exception
	{
		public CommandRequiresParameterException(string commandName) : base("Command '"+commandName+"' requires a parameter but none is given")
		{ }
	}



	public class CommandHasNoParameterException : Exception
	{
		public CommandHasNoParameterException(string commandName) : base("Command '"+commandName+"' does not support parameters, but a parameter is specified")
		{ }
	}

}
