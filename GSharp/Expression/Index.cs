using GSharp;
using GSharp.Expression;

public class Index : Expr, IToken
{
  public readonly Expr Indexee;
  public readonly Token ClosingBracket;
  public readonly Expr argument;

  public Index(Expr indexee, Token closingBracket, Expr argument)
  {
    this.Indexee = indexee;
    this.ClosingBracket = closingBracket;
    this.argument = argument;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitIndexExpr(this);
  }

  public override string ToString()
  {
    return $"{Indexee}[{argument}]";
  }

  public Token Token => ClosingBracket;
}