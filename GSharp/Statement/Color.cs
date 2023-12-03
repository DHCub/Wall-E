namespace GSharp.Statement;

public class ColorStmt : Stmt, IToken
{
  public readonly Token Command;
  public readonly Token Color;

  public ColorStmt(Token Command, Token Color)
  {
    this.Command = Command;
    this.Color = Color;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitColorStmt(this);
  }

  public Token Token => Color;
}