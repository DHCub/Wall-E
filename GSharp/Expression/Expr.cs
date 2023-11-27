namespace GSharp
{
  namespace Expr
  {
    public abstract class Expr
    {
      public interface IVisitor<R>
      {
        R VisitBinaryExpr(Binary expr);
        R VisitCallExpr(Call expr);
        R VisitConditionalExpr(Conditional expr);
        R VisitLetInExpr(LetIn expr);
        R VisitLiteralExpr(Literal expr);
        R VisitLogicalExpr(Logical expr);
        R VisitRangeExpr(Range expr);
        R VisitSequenceExpr(Sequence expr);
        R VisitUnaryExpr(Unary expr);
        R VisitUndefinedExpr(Undefined expr);
        R VisitVariableExpr(Variable expr);
      }

      public abstract R Accept<R>(IVisitor<R> visitor);
    }
  }
}