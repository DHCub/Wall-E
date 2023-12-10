using GSharp.Expression;

namespace GSharp.Statement;

public class Print : Stmt, IToken
{
  public readonly Token Command;
  public readonly Expr Expression;
  public readonly Token? Label;

  public Print(Token command, Expr expression, Token? label = null)
  {
    this.Command = command;
    this.Expression = expression;
    this.Label = label;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitPrintStmt(this);
  }

  public Token Token => Command;
}