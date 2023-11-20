namespace GSharp;

public class Draw : Stmt
{
  public readonly Expr elements;
  public readonly Token nameTk;
  public Draw(Expr elements, Token nameTk = null)
  {
    this.elements = elements;
    this.nameTk = nameTk;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitDrawStmt(this);
  }
}