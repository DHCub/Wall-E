namespace GSharp.Statement;

using GSharp.Expression;

public class Expression : Stmt
{
  public readonly Expr expression;

  public Expression(Expr expression)
  {
    this.expression = expression;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitExpressionStmt(this);
  }
}