namespace GSharp.Expression;

public abstract class Expr
{
  public ITypeReference TypeReference { get; }

  public Expr()
  {
    TypeReference = new TypeReference(TypeSpecifier: null);
  }

  public interface IVisitor<R>
  {
    R VisitAssignExpr(Assign expr);
    R VisitBinaryExpr(Binary expr);
    R VisitCallExpr(Call expr);
    R VisitConditionalExpr(Conditional expr);
    R VisitEmptyExpr(Empty expr);
    R VisitGroupingExpr(Grouping expr);
    R VisitIntRangeExpr(IntRange expr);
    R VisitLetInExpr(LetIn expr);
    R VisitLiteralExpr(Literal expr);
    R VisitLogicalExpr(Logical expr);
    R VisitSequenceExpr(Sequence expr);
    R VisitUnaryExpr(Unary expr);
    R VisitUndefinedExpr(Undefined expr);
    R VisitVariableExpr(Variable expr);
  }

  public abstract R Accept<R>(IVisitor<R> visitor);
}