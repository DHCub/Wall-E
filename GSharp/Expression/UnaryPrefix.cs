namespace GSharp.Expression;

public class UnaryPrefix : Expr, IToken
{
  public readonly Token Oper;
  public readonly Expr Right;

  public UnaryPrefix(Token oper, Expr right)
  {
    this.Oper = oper;
    this.Right = right;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitUnaryPrefixExpr(this);
  }

  public Token Token => Oper;
}