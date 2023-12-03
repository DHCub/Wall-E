namespace GSharp.Expression;

public class Conditional : Expr, IToken
{
  public readonly Token If;
  public readonly Expr Condition;
  public readonly Expr ThenBranch;
  public readonly Expr ElseBranch;

  public Conditional(Token If, Expr Condition, Expr ThenBranch, Expr ElseBranch)
  {
    this.Condition = Condition;
    this.ThenBranch = ThenBranch;
    this.ElseBranch = ElseBranch;
    this.If = If;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitConditionalExpr(this);
  }

  public Token Token => If;
}