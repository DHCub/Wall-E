namespace GSharp.Expression;

public class Logical : Expr, IToken
{
  public readonly Expr Left;
  public readonly Token Oper;
  public readonly Expr Right;

  public Logical(Expr Left, Token Oper, Expr Right)
  {
    this.Left = Left;
    this.Oper = Oper;
    this.Right = Right;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitLogicalExpr(this);
  }

  public Token Token => Oper;
}