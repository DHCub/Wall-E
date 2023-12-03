namespace GSharp.Expression;

public class Literal : Expr
{
  public readonly object? Value;

  public Literal(object Value)
  {
    this.Value = Value;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitLiteralExpr(this);
  }

  public override string ToString()
  {
    if (Value is string s)
    {
      return '"' + s + '"';
    }
    else
    {
      return Value?.ToString() ?? "undefined";
    }
  }
}