namespace GSharp.Statement;

using GSharp.Expression;

public class Draw : Stmt, IToken
{
  public readonly Token Command;
  public readonly Expr Elements;
  public readonly Token Label;

  public Draw(Token command, Expr elements, Token label = null)
  {
    this.Command = command;
    this.Elements = elements;
    this.Label = label;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitDrawStmt(this);
  }

  public Token Token => Command;
}