namespace GSharp.Expression;

public class Logical : Expr, IToken
{
  public readonly Expr Left;
  public readonly Token Oper;
  public readonly Expr Right;

  public Logical(Expr left, Token oper, Expr right)
  {
    this.Left = left;
    this.Oper = oper;
    this.Right = right;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitLogicalExpr(this);
  }

  public override string ToString()
  {
    return $"{Left} {Oper} {Right}";
  }

  public Token Token => Oper;
}