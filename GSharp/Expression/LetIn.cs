namespace GSharp;
using System.Collections.Generic;

public class LetIn : Expr
{
  public readonly List<Stmt> instructions;
  public readonly Expr body;

  public LetIn(List<Stmt> instructions, Expr body)
  {
    this.instructions = instructions;
    this.body = body;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitLetInExpr(this);
  }
}