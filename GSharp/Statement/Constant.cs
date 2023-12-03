using GSharp.Expression;
using System;
using System.Collections.Generic;

namespace GSharp.Statement;

public class ConstantStmt : Stmt, IToken
{
  public readonly List<Token> Names;
  public readonly Expr Initializer;

  public ConstantStmt(List<Token> Names, Expr Initializer)
  {
    this.Names = Names;
    this.Initializer = Initializer;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitConstantStmt(this);
  }

  public Token Token => Names.Count > 0 ? Names[0] : throw new ArgumentException("constant decl should have names");
}