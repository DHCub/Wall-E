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

  public override string ToString()
  {
    const string RUNTIME_ERROR = "! RUNTIME ERROR: ";
    
    if (token == null)
      return RUNTIME_ERROR + this.Message;
    
    return RUNTIME_ERROR + $"at {token.line}:{token.column}, " + this.Message;
  }
}