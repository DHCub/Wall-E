namespace GSharp.Expression;

public class IntRange : Expr
{
  public readonly Token left;
  public readonly Token dots;
  public readonly Token right;

  public IntRange(Token left, Token dots, Token right)
  {
    this.left = left;
    this.dots = dots;
    this.right = right;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitIntRangeExpr(this);
  }
}