namespace GSharp;

public class Call : Expr
{
  public readonly Expr calle;
  public readonly List<Expr> parameters;

  public Call(Expr calle, List<Expr> parameters)
  {
    this.calle = calle;
    this.parameters = parameters;
  }

  public int Arity => parameters.Count;

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitCallExpr(this);
  }
}