using System;

namespace GSharp.Exceptions;

public class RuntimeError : Exception
{
  public Token? token { get; }

  public RuntimeError(Token? token, string message) : base(message)
  {
    this.token = token;
  }

  public RuntimeError(Token? token, string message, Exception innerException) : base(message, innerException)
  {
    this.token = token;
  }
}