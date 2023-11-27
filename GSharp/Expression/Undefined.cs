namespace GSharp.Expression;

public class Undefined : Expr
{
  public Undefined() { }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitUndefinedExpr(this);
  }
}