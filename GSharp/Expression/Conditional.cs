namespace GSharp;

public class Conditional : Expr
{
  public readonly Expr condition;
  public readonly Expr thenBranch;
  public readonly Expr elseBranch;

  public Conditional(Expr condition, Expr thenBranch, Expr elseBranch)
  {
    this.condition = condition;
    this.thenBranch = thenBranch;
    this.elseBranch = elseBranch;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitConditionalExpr(this);
  }
}