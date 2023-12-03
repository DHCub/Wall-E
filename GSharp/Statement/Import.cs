namespace GSharp.Statement;

public class Import : Stmt, IToken
{
  public readonly Token Command;
  public readonly Token DirName;
  public Import(Token Command, Token DirName)
  {
    this.Command = Command;
    this.DirName = DirName;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitImportStmt(this);
  }

  public Token Token => Command;
}