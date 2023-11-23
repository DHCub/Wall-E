namespace GSharp;

public class Print : Stmt
{
  public readonly List<Expr> printe;

  public Print(List<Expr> printe)
  {
    this.printe = printe;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitPrintStmt(this);
  }
}