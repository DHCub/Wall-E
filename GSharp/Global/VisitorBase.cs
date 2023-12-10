using System;
using System.Collections.Generic;
using GSharp.Expression;
using GSharp.Statement;

namespace GSharp;

/// <summary>
/// Abstract base class for expression visitors.
///
/// All the methods implemented in this class are no-op, but they do their best to try
/// and make the whole tree be traversed. The idea is to give child classes an opportunity
/// to only override a smaller subset of the full set of expression types, to avoid 
/// unnecessary boilerplate code.
/// </summary>
public abstract class VisitorBase : Expr.IVisitor<VoidObject>, Stmt.IVisitor<VoidObject>
{
  protected void Visit(IEnumerable<Stmt> statements)
  {
    foreach (var statement in statements)
    {
      Visit(statement);
    }
  }

  public void Visit(Stmt stmt)
  {
    stmt.Accept(this);
  }

  public void Visit(Expr expr)
  {
    expr.Accept(this);
  }

  public virtual VoidObject VisitAssignExpr(Assign expr)
  {
    Visit(expr.Value);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitBlockStmt(Block stmt)
  {
    Visit(stmt.Statements);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitBinaryExpr(Binary expr)
  {
    Visit(expr.Left);
    Visit(expr.Right);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitCallExpr(Call expr)
  {
    Visit(expr.Calle);

    foreach (var argument in expr.Arguments)
    {
      Visit(argument);
    }

    return VoidObject.Void;
  }

  public virtual VoidObject VisitColorStmt(ColorStmt stmt)
  {
    return VoidObject.Void;
  }

  public virtual VoidObject VisitConditionalExpr(Conditional expr)
  {
    Visit(expr.Condition);
    Visit(expr.ThenBranch);
    Visit(expr.ElseBranch);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitConstantStmt(ConstantStmt stmt)
  {
    Visit(stmt.Initializer);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitDrawStmt(Draw stmt)
  {
    return VoidObject.Void;
  }

  public virtual VoidObject VisitEmptyExpr(Empty expr)
  {
    return VoidObject.Void;
  }

  public virtual VoidObject VisitExpressionStmt(ExpressionStmt stmt)
  {
    Visit(stmt.Expression);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitFunctionStmt(Function stmt)
  {
    Visit(stmt.Body);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitGroupingExpr(Grouping expr)
  {
    Visit(expr.Expression);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitImportStmt(Import stmt)
  {
    return VoidObject.Void;
  }

  public virtual VoidObject VisitIntRangeExpr(IntRange expr)
  {
    return VoidObject.Void;
  }

  public virtual VoidObject VisitIndexExpr(Index expr)
  {
    Visit(expr.Indexee);
    Visit(expr.Argument);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitLetInExpr(LetIn expr)
  {
    Visit(expr.Stmts);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitLiteralExpr(Literal expr)
  {
    return VoidObject.Void;
  }

  public virtual VoidObject VisitLogicalExpr(Logical expr)
  {
    Visit(expr.Left);
    Visit(expr.Right);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitPrintStmt(Print stmt)
  {
    Visit(stmt.Expression);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitRestoreStmt(Restore stmt)
  {
    return VoidObject.Void;
  }

  public virtual VoidObject VisitSequenceExpr(Sequence expr)
  {
    foreach (var items in expr.Items)
    {
      Visit(items);
    }

    return VoidObject.Void;
  }

  public virtual VoidObject VisitUnaryPrefixExpr(UnaryPrefix expr)
  {
    Visit(expr.Right);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitUnaryPostfixExpr(UnaryPostfix expr)
  {
    Visit(expr.Left);

    return VoidObject.Void;
  }

  public virtual VoidObject VisitUndefinedExpr(Undefined expr)
  {
    return VoidObject.Void;
  }

  public virtual VoidObject VisitVariableExpr(Variable expr)
  {
    return VoidObject.Void;
  }

  public virtual VoidObject VisitReturnStmt(Statement.Return stmt)
  {
    if (stmt.Value != null)
    {
      Visit(stmt.Value);
    }

    return VoidObject.Void;
  }

  public virtual VoidObject VisitVarStmt(Var stmt)
  {
    if (stmt.Initializer is not null)
    {
      Visit(stmt.Initializer);
    }

    return VoidObject.Void;
  }

  public virtual VoidObject VisitWhileStmt(While stmt)
  {
    Visit(stmt.Condition);
    Visit(stmt.Body);

    return VoidObject.Void;
  }
}