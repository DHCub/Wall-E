using GSharp.Expression;
using GSharp.Statement;

public class While : Stmt
{
  public readonly Expr Condition;
  public readonly Stmt Body;

  public While(Expr condition, Stmt body)
  {
    this.Condition = condition;
    this.Body = body;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitWhileStmt(this);
  }
}