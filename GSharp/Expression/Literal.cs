namespace GSharp;

public class Literal : Expr
{
  public readonly object value;
  public Literal(object value)
  {
    this.value = value;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitLiteralExpr(this);
  }
}