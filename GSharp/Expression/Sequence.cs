using System.Collections.Generic;

namespace GSharp.Expression;

public class Sequence : Expr, IToken
{
  public readonly Token Brace;
  public readonly List<Expr> Items;

  public Sequence(Token brace, List<Expr> items)
  {
    this.Items = items;
    this.Brace = brace;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitSequenceExpr(this);
  }

  public Token Token => Brace;
}