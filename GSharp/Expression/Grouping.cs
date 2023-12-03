namespace GSharp.Expression;

public class Grouping : Expr, IToken
{
  public readonly Expr Expression;

  public Grouping(Expr Expression)
  {
    this.Expression = Expression;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitGroupingExpr(this);
  }

  public Token? Token => (Expression as IToken)?.Token;
}