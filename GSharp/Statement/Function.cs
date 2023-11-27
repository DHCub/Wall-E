namespace GSharp.Statement;

using GSharp.Expression;
using System.Collections.Generic;

public class Function : Stmt
{
  public readonly Token name;
  public readonly List<Token> parameters;
  public readonly Expr body;

  public Function(Token name, List<Token> parameters, Expr body)
  {
    this.name = name;
    this.parameters = parameters;
    this.body = body;
  }

  public int Arity => parameters.Count;

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitFunctionStmt(this);
  }
}