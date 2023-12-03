using System;

namespace GSharp;

public class Token
{
  public TokenType type { get; private set; }
  public string lexeme { get; private set; }
  public object literal { get; private set; }
  public int line { get; private set; }
  public int column { get; private set; }

  public Token(TokenType type, string lexeme, object literal, int line, int column)
  {
    this.type = type;
    this.lexeme = lexeme ?? throw new ArgumentException("lexeme cannot be null");
    this.literal = literal;
    this.line = line;
    this.column = column;
  }

  public override string ToString()
  {
    if (literal != null)
    {
      return $"Token({type}, {lexeme}, {literal})";
    }
    else
    {
      return $"Token({type}, {lexeme})";
    }
  }
}