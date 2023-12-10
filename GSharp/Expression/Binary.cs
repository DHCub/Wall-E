namespace GSharp.Expression;

public class Binary : Expr, IToken
{
  public readonly Expr Left;
  public readonly Token Oper;
  public readonly Expr Right;

  public Binary(Expr left, Token oper, Expr right)
  {
    this.Left = left;
    this.Oper = oper;
    this.Right = right;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitBinaryExpr(this);
  }

  public override string ToString()
  {
    return $"{Left} {Oper} {Right}";
  }

  public Token Token => Oper;
}