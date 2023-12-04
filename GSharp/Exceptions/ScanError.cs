using System;

namespace GSharp.Exceptions;

public class ScanError : Exception
{
  public int line { get; }

  public ScanError(string message, int line) : base(message)
  {
    this.line = line;
  }
}