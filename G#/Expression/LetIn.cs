namespace GSharp;

public class LetIn : Expr
{
  public readonly List<Assign> instructions;
  public readonly Expr body;

  public LetIn(List<Assign> instructions, Expr body)
  {
    this.instructions = instructions;
    this.body = body;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitLetInExpr(this);
  }
}