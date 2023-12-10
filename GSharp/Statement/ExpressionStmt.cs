using GSharp.Expression;

namespace GSharp.Statement;

public class ExpressionStmt : Stmt
{
  public readonly Expr Expression;

  public ExpressionStmt(Expr Expression)
  {
    this.Expression = Expression;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitExpressionStmt(this);
  }
}