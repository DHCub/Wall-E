using System;
using System.Collections.Generic;

namespace GSharp.Exceptions;

public class RuntimeError : Exception
{
  public Token? token { get; }
  private Stack<string> StackTrace;

  public RuntimeError(Token? token, string message, Stack<string> stackTrace) : base(message)
  {
    this.token = token;
    this.StackTrace = stackTrace;
  }

  public RuntimeError(Token? token, string message, Stack<string> stackTrace, Exception innerException) : base(message, innerException)
  {
    this.token = token;
    this.StackTrace = stackTrace;
  }

  public void AddStackTrace(Stack<string> trace) {this.StackTrace = trace;}
  public override string ToString()
  {
    const string RUNTIME_ERROR = "! RUNTIME ERROR: ";
    List<char> answ = new();
    answ.AddRange(RUNTIME_ERROR);
    
    if (token != null)
      answ.AddRange($"at {token.line}:{token.column}, ");
    
    answ.AddRange(this.Message);

    answ.AddRange(StackTraceBuilder.GetStackTrace(StackTrace));

    return new string(answ.ToArray());
  }
}