using System;
using System.Collections.Generic;

namespace GSharp.Exceptions;

public class RuntimeError : Exception
{
  public Token? token { get; }
  private Stack<string> ImportTrace;

  public RuntimeError(Token? token, string message, Stack<string> importTrace) : base(message)
  {
    this.token = token;
    this.ImportTrace = importTrace;
  }

  public RuntimeError(Token? token, string message, Stack<string> importTrace ,Exception innerException) : base(message, innerException)
  {
    this.token = token;
    this.ImportTrace = importTrace;
  }

  public void AddImportTrace(Stack<string> trace) {this.ImportTrace = trace;}
  public override string ToString()
  {
    const string RUNTIME_ERROR = "! RUNTIME ERROR: ";
    List<char> answ = new();
    answ.AddRange(RUNTIME_ERROR);
    
    if (token != null)
      answ.AddRange($"at {token.line}:{token.column}, ");
    
    answ.AddRange(this.Message);

    if (ImportTrace != null && ImportTrace.Count > 0)
    {
      bool first = true;
      foreach(var file in ImportTrace)
      {
        if (first)
        {
          answ.AddRange("\nAt file: \n");
          answ.AddRange(file);
          first = false;
        }
        else
        {
          answ.AddRange(", Imported from File:\n");
          answ.AddRange(file); 
        }
      }
    }

    return new string(answ.ToArray());
  }
}