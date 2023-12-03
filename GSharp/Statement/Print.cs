using GSharp.Expression;

namespace GSharp.Statement;

public class Print : Stmt, IToken
{
  public readonly Token Command;
  public readonly Expr Expression;
  public readonly Token? Label;

  public Print(Token Command, Expr Expression, Token? Label = null)
  {
    this.Command = Command;
    this.Expression = Expression;
    this.Label = Label;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitPrintStmt(this);
  }

  public Token Token => Command;
}