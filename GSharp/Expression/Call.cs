using System;
using System.Collections.Generic;

namespace GSharp.Expression;

public class Call : Expr, IToken
{
  public readonly Expr Calle;
  public IToken TokenAwareCalle => (IToken)Calle;
  public readonly Token Paren;
  public readonly List<Expr> Arguments;

  public string? CalleToString
  {
    get
    {
      if (Calle is Variable variable)
      {
        return variable.Name.lexeme;
      }
      else
      {
        return ToString();
      }
    }
  }

  public Call(Expr Calle, Token Paren, List<Expr> Arguments)
  {
    if (Calle is not IToken)
    {
      throw new ArgumentException("Calle must be IToken");
    }

    this.Calle = Calle;
    this.Paren = Paren;
    this.Arguments = Arguments;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitCallExpr(this);
  }

  public override string ToString()
  {
    if (Calle is Variable variable)
    {
      return $"'call function {variable.Name.lexeme}'";
    }
    else
    {
      return base.ToString();
    }
  }

  public Token Token => Paren;
}