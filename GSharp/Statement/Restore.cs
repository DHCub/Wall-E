namespace GSharp.Statement;

public class Restore : Stmt, IToken
{
  public readonly Token Command;

  public Restore(Token command)
  {
    this.Command = command;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitRestoreStmt(this);
  }

  public Token Token => Command;
}