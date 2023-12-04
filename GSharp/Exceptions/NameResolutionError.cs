using System;
using static GSharp.TokenType;

namespace GSharp.Exceptions;

// <summary>
// Emmited for name resolution errors which can be detected at an early stage,
// before type validation is even attempted.
// </summary>

public class NameResolutionError : Exception
{
  public Token Token { get; }

  public NameResolutionError(string message, Token token) : base(message)
  {
    this.Token = token;
  }

  public override string ToString()
  {
    string where;

    if (Token.type == EOF)
    {
      where = " at end";
    }
    else
    {
      where = " at '" + Token.lexeme + "'";
    }

    return $"[line {Token.line}] Error{where}: {Message}";
  }
}