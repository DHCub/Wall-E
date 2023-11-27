namespace GSharp.Expression;

public class Range : Expr
{
  public readonly Token left;
  public readonly Token right;

  public Range(Token left, Token right)
  {
    this.left = left;
    this.right = right;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitRangeExpr(this);
  }
}