namespace GSharp;
using System.Collections.Generic;

public class Sequence : Expr
{
  public readonly Token openBraceTk;
  public readonly List<Expr> items;

  public Sequence(Token openBraceTk,  List<Expr> items)
  {
    this.items = items;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitSequenceExpr(this);
  }
}