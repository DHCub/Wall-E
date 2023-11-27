using System.Collections.Generic;

namespace GSharp
{
  namespace Expr
  {
    public class LetIn : Expr
    {
      public readonly Token letTk;
      public readonly List<Stmt> instructions;
      public readonly Expr body;

      public LetIn(Token letTk, List<Stmt> instructions, Expr body)
      {
        this.instructions = instructions;
        this.body = body;
        this.letTk = letTk;
      }

      public override R Accept<R>(IVisitor<R> visitor)
      {
        return visitor.VisitLetInExpr(this);
      }
    }
  }
}