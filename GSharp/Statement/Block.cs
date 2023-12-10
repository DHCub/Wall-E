using System.Collections.Generic;
using GSharp.Statement;

public class Block : Stmt
{
  public readonly List<Stmt> Statements;

  public Block(List<Stmt> statements)
  {
    this.Statements = statements;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitBlockStmt(this);
  }
}