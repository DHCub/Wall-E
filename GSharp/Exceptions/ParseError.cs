using System;

namespace GSharp.Exceptions;

using static TokenType;

public class ParseError : Exception
{
  public Token token { get; }
  public ParseErrorType? parseErrorType { get; }

  public ParseError(string message, Token token, ParseErrorType? errorType) : base(message)
  {
    this.token = token;
    this.parseErrorType = errorType;
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

    return $"! PARSE ERROR: at line {token.line}: Error{where}: {Message}";
  }
}