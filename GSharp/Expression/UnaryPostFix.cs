using GSharp;
using GSharp.Expression;

public class UnaryPostfix : Expr, IToken
{
  public readonly Expr Left;
  public readonly Token Name;
  public readonly Token Oper;

  public UnaryPostfix(Expr left, Token name, Token oper)
  {
    this.Left = left;
    this.Name = name;
    this.Oper = oper;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitUnaryPostfixExpr(this);
  }

  public Token Token => Oper;
}