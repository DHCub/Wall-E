namespace GSharp;

public class Restore : Stmt
{
  public Restore() { }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitRestoreStmt(this);
  }
}