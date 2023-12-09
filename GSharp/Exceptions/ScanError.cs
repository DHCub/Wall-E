using System;
using System.Collections.Generic;

namespace GSharp.Exceptions;

public class ScanError : Exception
{
  public int line { get; }
  private Stack<string> importTrace;

  public ScanError(string message, int line, Stack<string> importTrace) : base(message)
  {
    this.line = line;
    this.importTrace = importTrace;
  }

  public override string ToString()
    => "! SCAN ERROR: " + "at line " + line + ": " + base.Message + ImportTraceBuilder.GetImportTraceString(importTrace);
}