namespace GSharp.Expression;

using System.Collections.Generic;

public class Call : Expr
{
  public readonly Expr calle;
  public readonly Token paren;
  public readonly List<Expr> parameters;

  public Call(Expr calle, Token paren, List<Expr> parameters)
  {
    this.calle = calle;
    this.paren = paren;
    this.parameters = parameters;
  }

  public int Arity => parameters.Count;

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitCallExpr(this);
  }
}