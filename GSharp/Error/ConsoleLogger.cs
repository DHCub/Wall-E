namespace GSharp;
using System;

public class ConsoleLogger : ILogger
{
  public bool hadError { get; private set; }

  public bool hadRuntimeError { get; private set; }

  public void Error(string type, int line, int column, string where, string message)
  {
    Console.WriteLine($"! {type} ERROR [{line}:{column}] {where}: {message}");
    hadError = true;
  }

  public void RuntimeError(RuntimeError error)
  {
    Console.WriteLine($"! RUNTIME ERROR: {error.Message}");
    hadRuntimeError = true;
  }

  public void ResetError()
  {
    hadError = false;
  }

  public void ResetRuntimeError()
  {
    hadRuntimeError = false;
  }
}
