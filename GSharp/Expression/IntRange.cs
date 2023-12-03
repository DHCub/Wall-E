namespace GSharp.Expression;

public class IntRange : Expr, IToken
{
  public readonly Token Left;
  public readonly Token Dots;
  public readonly Token Right;

  public IntRange(Token Left, Token Dots, Token Right)
  {
    this.Left = Left;
    this.Dots = Dots;
    this.Right = Right;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitIntRangeExpr(this);
  }

  public Token Token => Dots;
}