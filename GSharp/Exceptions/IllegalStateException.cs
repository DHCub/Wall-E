using System.Collections.Generic;

namespace GSharp.Exceptions;

public class IllegalStateException : ObjectException
{
  public IllegalStateException(string message, Stack<string> importTrace) : base(message, importTrace) { }
}