namespace GSharp;

public class Assign : Expr
{
  public readonly List<Token> name;
  public readonly Expr value;

  public Assign(List<Token> name, Expr value)
  {
    this.name = name;
    this.value = value;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitAssignExpr(this);
  }
}