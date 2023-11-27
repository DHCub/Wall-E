namespace GSharp.Expression;

public class Logical : Expr
{
  public readonly Expr left;
  public readonly Token oper;
  public readonly Expr right;

  public Logical(Expr left, Token oper, Expr right)
  {
    this.left = left;
    this.oper = oper;
    this.right = right;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitLogicalExpr(this);
  }
}