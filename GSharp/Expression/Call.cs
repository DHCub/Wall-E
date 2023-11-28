namespace GSharp.Expression;

using System.Collections.Generic;

public class Call : Expr
{
  public readonly Expr calle;
  public readonly Token paren;
  public readonly List<Expr> arguments;

  public Call(Expr calle, Token paren, List<Expr> arguments)
  {
    this.calle = calle;
    this.paren = paren;
    this.arguments = arguments;
  }

  public int Arity => arguments.Count;

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitCallExpr(this);
  }
}