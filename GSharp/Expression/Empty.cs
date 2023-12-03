namespace GSharp.Expression;

public class Empty : Expr
{
  public Empty() : base() { }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitEmptyExpr(this);
  }
}