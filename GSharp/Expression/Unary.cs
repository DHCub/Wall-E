namespace GSharp
{
  namespace Expr
  {
    public class Unary : Expr
    {
      public readonly Token oper;
      public readonly Expr right;

      public Unary(Token oper, Expr right)
      {
        this.oper = oper;
        this.right = right;
      }

      public override R Accept<R>(IVisitor<R> visitor)
      {
        return visitor.VisitUnaryExpr(this);
      }
    }
  }
}