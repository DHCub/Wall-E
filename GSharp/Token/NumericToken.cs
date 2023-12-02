namespace GSharp;

using static TokenType;

public class NumericToken : Token
{
  public bool isFractional { get; }

  public NumericToken(string lexeme, object literal, int line, int column, bool isFractional) : base(NUMBER, lexeme, literal, line, column)
  {
    this.isFractional = isFractional;
  }
}