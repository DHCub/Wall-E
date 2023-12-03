using System.Collections.Generic;

namespace GSharp.Expression;

public class Sequence : Expr, IToken
{
  public readonly Token Brace;
  public readonly List<Expr> Items;

  public Sequence(Token Brace, List<Expr> Items)
  {
    this.Items = Items;
    this.Brace = Brace;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitSequenceExpr(this);
  }

  public Token Token => Brace;
}