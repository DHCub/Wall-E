namespace GSharp.Statement;

public class Import : Stmt
{
  public readonly Token dirName;
  public Import(Token dirName)
  {
    this.dirName = dirName;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitImportStmt(this);
  }
}