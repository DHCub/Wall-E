namespace GSharp.Statement;

using GSharp.Expression;

using System.Collections.Generic;

public class Constant : Stmt
{
  public readonly List<Token> constNames;
  public readonly Expr initializer;

  public Constant(List<Token> constNames, Expr initializer)
  {
    this.constNames = constNames;
    this.initializer = initializer;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitConstantStmt(this);
  }
}