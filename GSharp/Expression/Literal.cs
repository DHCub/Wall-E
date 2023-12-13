namespace GSharp.Expression;

public class Literal : Expr
{
  public readonly object? Value;

  public Literal(object value)
  {
    this.Value = value;
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