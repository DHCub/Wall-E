using System.Collections.Generic;
using GSharp.Statement;

namespace GSharp.Expression;

public class LetIn : Expr, IToken
{
  public readonly Token Let;
  public readonly List<Stmt> Stmts;

  public LetIn(Token let, List<Stmt> stmts)
  {
    this.Stmts = stmts;
    this.Let = let;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitLetInExpr(this);
  }

  public Token Token => Let;
}