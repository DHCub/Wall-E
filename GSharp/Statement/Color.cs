namespace GSharp.Statement;

public class Color : Stmt
{
  public readonly Token color;

  public Color(Token color)
  {
    this.color = color;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitColorStmt(this);
  }
}