namespace GSharp.Expression;

public class Unary : Expr, IToken
{
  public readonly Token Oper;
  public readonly Expr Right;

  public Unary(Token Oper, Expr Right)
  {
    this.Oper = Oper;
    this.Right = Right;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitUnaryExpr(this);
  }

  public Token Token => Oper;
}