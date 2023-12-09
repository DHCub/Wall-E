using System;
using System.Collections.Generic;

namespace GSharp.Exceptions;

using static TokenType;

public class ParseError : Exception
{
  public Token token { get; }
  public ParseErrorType? parseErrorType { get; }
  private Stack<string> importTrace;

  public ParseError(string message, Token token, Stack<string> importTrace,ParseErrorType? errorType) : base(message)
  {
    this.token = token;
    this.parseErrorType = errorType;
    this.importTrace = importTrace;
  }

  public override string ToString()
  {
    string where;

    if (token.type == EOF)
    {
      where = " at end";
    }
    else
    {
      where = " at '" + token.lexeme + "'";
    }

    var answ = $"! PARSE ERROR: at line {token.line}: Error{where}: {Message}";
    answ += ImportTraceBuilder.GetImportTraceString(importTrace);

    return answ;
  }
}