using GSharp.Expression;

namespace GSharp.Statement;

public class Return : Stmt
{
  public Token? Keyword { get; }
  public Expr Value { get; }

  public Return(Token? keyword, Expr Value)
  {
    this.Keyword = keyword;
    this.Value = Value;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitReturnStmt(this);
  }
}