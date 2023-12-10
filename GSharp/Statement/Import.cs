namespace GSharp.Statement;

public class Import : Stmt, IToken
{
  public readonly Token Command;
  public readonly Token DirName;
  public Import(Token command, Token dirName)
  {
    this.Command = command;
    this.DirName = dirName;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitImportStmt(this);
  }

  public Token Token => Command;
}