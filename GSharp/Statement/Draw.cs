namespace GSharp;

public class Draw : Stmt
{
  public readonly Token commandTk;
  public readonly Expr elements;
  public readonly Token nameTk;
  public Draw(Expr elements, Token commandTk, Token nameTk = null)
  {
    this.elements = elements;
    this.nameTk = nameTk;
    this.commandTk = commandTk;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitDrawStmt(this);
  }
}