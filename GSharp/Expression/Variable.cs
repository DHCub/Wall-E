namespace GSharp.Expression;

public class Variable : Expr, IToken
{
  public readonly Token Name;

  public Variable(Token Name)
  {
    this.Name = Name;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitVariableExpr(this);
  }

  public override string ToString()
  {
    return $"#<Variable {Name.lexeme}>";
  }

  public Token Token => Name;
}