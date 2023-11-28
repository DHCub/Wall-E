namespace GSharp.Statement;

using GSharp.Expression;

public class Var : Stmt
{
  public readonly Token type;
  public readonly Token name;
  public readonly Expr initializer;
  public readonly bool isSequence;

  public Var(Token type, Token name, Expr initializer, bool isSequence = false)
  {
    this.type = type;
    this.name = name;
    this.initializer = initializer;
    this.isSequence = isSequence;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitVarStmt(this);
  }
}