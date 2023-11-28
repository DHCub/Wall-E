using GSharp.Expression;

namespace GSharp.Expression;

public class Grouping : Expr
{
  public readonly Expr expression;
  public Grouping(Expr expression)
  {
    this.expression = expression;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitGroupingExpr(this);
  }
}