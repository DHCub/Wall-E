namespace GSharp
{
  namespace Expression
  {
    public class Conditional : Expr
    {
      public readonly Token ifTk;
      public readonly Expr condition;
      public readonly Expr thenBranch;
      public readonly Expr elseBranch;

      public Conditional(Token ifTk, Expr condition, Expr thenBranch, Expr elseBranch)
      {
        this.condition = condition;
        this.thenBranch = thenBranch;
        this.elseBranch = elseBranch;
        this.ifTk = ifTk;
      }

      public override R Accept<R>(IVisitor<R> visitor)
      {
        return visitor.VisitConditionalExpr(this);
      }
    }
  }
}