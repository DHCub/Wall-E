namespace GSharp
{
  namespace Expr
  {
    public class Undefined : Expr
    {
      public Undefined() { }

      public override R Accept<R>(IVisitor<R> visitor)
      {
        return visitor.VisitUndefinedExpr(this);
      }
    }
  }
}