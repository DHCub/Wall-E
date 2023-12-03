namespace GSharp.Statement;

using GSharp.Expression;

public class Draw : Stmt, IToken
{
  public readonly Token Command;
  public readonly Expr Elements;
  public readonly Token Label;

  public Draw(Token Command, Expr Elements, Token Label = null)
  {
    this.Command = Command;
    this.Elements = Elements;
    this.Label = Label;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitDrawStmt(this);
  }

  public Token Token => Command;
}