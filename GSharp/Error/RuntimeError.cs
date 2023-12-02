namespace GSharp;
using System;
public class RuntimeError : Exception
{
  public RuntimeError(string message) : base(message) {}
}