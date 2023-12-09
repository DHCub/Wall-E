using System.Collections.Generic;

namespace GSharp.Exceptions;

public abstract class ObjectException : RuntimeError
{
  protected ObjectException(string message, Stack<string> importTrace) : base(null, message, importTrace) { }
}