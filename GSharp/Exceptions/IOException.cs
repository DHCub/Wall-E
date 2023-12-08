using System.Collections.Generic;

namespace GSharp.Exceptions;

public class IOExceptions : ObjectException
{
  public int errorNumber { get; }

  public IOExceptions(string message, int errorNumber, Stack<string> importTrace) : base($"Error {errorNumber}: {message}", importTrace)
  {
    this.errorNumber = errorNumber;
  }
}