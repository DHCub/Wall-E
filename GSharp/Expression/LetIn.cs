using System.Collections.Generic;
using GSharp.Statement;

namespace GSharp.Expression;

public class LetIn : Expr, IToken
{
  public readonly Token Let;
  public readonly List<Stmt> Stmts;
  public readonly Expr Body;

  public LetIn(Token Let, List<Stmt> Stmts, Expr Body)
  {
    this.Stmts = Stmts;
    this.Body = Body;
    this.Let = Let;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitLetInExpr(this);
  }

  public Token Token => Let;
}