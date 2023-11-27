namespace GSharp
{
  namespace Expression
  {
    public class Binary : Expr
    {
      public readonly Expr left;
      public readonly Token oper;
      public readonly Expr right;

      public Binary(Expr left, Token oper, Expr right)
      {
        this.left = left;
        this.oper = oper;
        this.right = right;
      }

      public override R Accept<R>(IVisitor<R> visitor)
      {
        return visitor.VisitBinaryExpr(this);
      }
    }
  }
}