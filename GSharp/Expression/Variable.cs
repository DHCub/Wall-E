namespace GSharp
{
  namespace Expression
  {
    public class Variable : Expr
    {
      public readonly Token name;

      public Variable(Token name)
      {
        this.name = name;
      }

      public override R Accept<R>(IVisitor<R> visitor)
      {
        return visitor.VisitVariableExpr(this);
      }
    }
  }
}