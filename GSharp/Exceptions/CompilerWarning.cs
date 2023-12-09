using System;
using static GSharp.TokenType;

namespace GSharp.Exceptions;

/// <summary>
/// Represent a compiler warning.
/// </summary>
public class CompilerWarning : Exception
{
  public Token Token { get; }
  public WarningType WarningType { get; }

  public CompilerWarning(string message, Token token, WarningType warningType) : base(message)
  {
    this.Token = token;
    this.WarningType = warningType;
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

    return $"[line {Token.line}] Warning{where}: {Message}";
  }
}