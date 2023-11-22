namespace GSharp;

public class Sequence : Expr
{
  public readonly List<Expr> items;

  public Sequence(List<Expr> items)
  {
    this.items = items;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitSequenceExpr(this);
  }
}