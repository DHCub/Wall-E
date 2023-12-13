using GSharp;
using GSharp.Expression;

public class Assign : Expr, IToken
{
  public readonly Variable Variable;
  public readonly Expr Value;

  public Assign(Variable variable, Expr value)
  {
    this.Variable = variable;
    this.Value = value;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitAssignExpr(this);
  }

  public override string ToString()
  {
    return $"{Variable} := {Value}";
  }

  public Token Name => Variable.Name;

  public Token Token => Name;
}