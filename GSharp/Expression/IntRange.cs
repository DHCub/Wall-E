namespace GSharp.Expression;

public class IntRange : Expr, IToken
{
  public readonly Token Left;
  public readonly Token Dots;
  public readonly Token Right;
  public readonly bool LeftNegative, RightNegative;

  public IntRange(Token left, Token dots, Token right, bool leftNegative = false, bool rightNegative = false)
  {
    this.Left = left;
    this.Dots = dots;
    this.Right = right;
    this.LeftNegative = leftNegative;
    this.RightNegative = rightNegative;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitIntRangeExpr(this);
  }

  public override string ToString()
  {
    if (Right is null)
    {
      return $"{(LeftNegative ? "-" : "") + Left.lexeme} ... oo";
    }
    else
    {
      return $"{(LeftNegative ? "-" : "") + Left.lexeme} ... {(RightNegative ? "-" : "") + Right.lexeme}";
    }
  }

  public Token Token => Dots;
}