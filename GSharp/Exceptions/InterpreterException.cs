using System;

namespace GSharp.Exceptions;

public class InterpreterException : Exception
{
  public InterpreterException(string message) : base(message)
  {

  }
}