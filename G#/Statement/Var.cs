namespace GSharp;

public class Var : Stmt
{
  public readonly Token type;
  public readonly Token name;
  public readonly Stmt initializer;
  public readonly bool isSequence;

  public Var(Token type, Token name, Stmt initializer, bool isSequence = true)
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