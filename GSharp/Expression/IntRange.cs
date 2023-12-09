namespace GSharp.Expression;

public class IntRange : Expr, IToken
{
  public readonly Token Left;
  public readonly Token Dots;
  public readonly Token Right;
  public readonly bool LeftNegative, RightNegative;

  public IntRange(Token Left, Token Dots, Token Right, bool LeftNegative = false, bool RightNegative = false)
  {
    this.Left = Left;
    this.Dots = Dots;
    this.Right = Right;
    this.LeftNegative = LeftNegative;
    this.RightNegative = RightNegative;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitIntRangeExpr(this);
  }

  public Token Token => Dots;
}