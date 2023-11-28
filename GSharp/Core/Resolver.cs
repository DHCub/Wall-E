namespace GSharp.Core;

using System.Collections;
using System.Collections.Generic;
using GSharp.Expression;
using GSharp.Statement;

public class Resolver : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
  private readonly Interpreter interpreter;
  private readonly Stack<Dictionary<string, bool>> scopes;

  public readonly ILogger logger;
  private FunctionType currentFunction = FunctionType.NONE;

  private enum FunctionType
  {
    NONE, FUNCTION, INITIALIZER
  }

  public Resolver(Interpreter interpreter, ILogger logger)
  {
    this.interpreter = interpreter;
  }

  public void Resolve(List<Stmt> statements)
  {
    foreach (var statement in statements)
    {
      Resolve(statement);
    }
  }

  public object VisitBinaryExpr(Binary expr)
  {
    Resolve(expr.left);
    Resolve(expr.right);
    return null;
  }

  public object VisitCallExpr(Call expr)
  {
    Resolve(expr.calle);

    foreach (var argument in expr.arguments)
    {
      Resolve(argument);
    }

    return null;
  }

  public object VisitColorStmt(Color stmt)
  {
    Resolve(stmt);
    return null;
  }

  public object VisitConditionalExpr(Conditional expr)
  {
    Resolve(expr.condition);
    Resolve(expr.thenBranch);
    Resolve(expr.elseBranch);
    return null;
  }

  public object VisitConstantStmt(Constant stmt)
  {
    throw new System.NotImplementedException();
  }

  public object VisitDrawStmt(Draw stmt)
  {
    Resolve(stmt);
    return null;
  }

  public object VisitExpressionStmt(Expression stmt)
  {
    Resolve(stmt.expression);
    return null;
  }

  public object VisitFunctionStmt(Function stmt)
  {
    Declare(stmt.name);
    Define(stmt.name);

    ResolveFunction(stmt, FunctionType.FUNCTION);
    return null;
  }

  public object VisitGroupingExpr(Grouping expr)
  {
    Resolve(expr.expression);
    return null;
  }

  public object VisitImportStmt(Import stmt)
  {
    return null;
  }

  public object VisitIntRangeExpr(IntRange expr)
  {
    throw new System.NotImplementedException();
  }

  public object VisitLetInExpr(LetIn expr)
  {
    throw new System.NotImplementedException();
  }

  public object VisitLiteralExpr(Literal expr)
  {
    return null;
  }

  public object VisitLogicalExpr(Logical expr)
  {
    Resolve(expr.left);
    Resolve(expr.right);
    return null;
  }

  public object VisitPrintStmt(Print stmt)
  {
    foreach (var expr in stmt.printe)
    {
      Resolve(expr);
    }
    return null;
  }

  public object VisitRestoreStmt(Restore stmt)
  {
    return null;
  }

  public object VisitSequenceExpr(Sequence expr)
  {
    throw new System.NotImplementedException();
  }

  public object VisitUnaryExpr(Unary expr)
  {
    Resolve(expr.right);
    return null;
  }

  public object VisitUndefinedExpr(Undefined expr)
  {
    return null;
  }

  public object VisitVariableExpr(Variable expr)
  {
    if (scopes.Count > 0 && scopes.Peek().ContainsKey(expr.name.lexeme))
    {
      if (!scopes.Peek()[expr.name.lexeme])
      {
        logger.Error("", expr.name, "Can't read local variable in its own initializer.");
      }
    }

    ResolveLocal(expr, expr.name);
    return null;
  }

  public object VisitVarStmt(Var stmt)
  {
    Declare(stmt.name);
    if (stmt.initializer is not null)
    {
      Resolve(stmt.initializer);
    }
    Define(stmt.name);
    return null;
  }

  private void Resolve(Stmt stmt)
  {
    stmt.Accept(this);
  }

  private void Resolve(Expr expr)
  {
    expr.Accept(this);
  }

  private void ResolveFunction(Function fun, FunctionType type)
  {
    FunctionType enclosingFunction = currentFunction;
    currentFunction = type;

    BeginScope();

    foreach (var param in fun.parameters)
    {
      Declare(param);
      Define(param);
    }

    Resolve(fun.body);

    EndScope();

    currentFunction = enclosingFunction;
  }

  private void BeginScope()
  {
    scopes.Push(new Dictionary<string, bool>());
  }

  private void EndScope()
  {
    scopes.Pop();
  }

  private void Declare(Token name)
  {
    if (scopes.Count == 0) return;

    var scope = scopes.Peek();

    if (scope.ContainsKey(name.lexeme))
    {
      logger.Error("", name, "Already a variable with this name in this scope.");
    }

    scope.Add(name.lexeme, false);
  }

  private void Define(Token name)
  {
    if (scopes.Count == 0) return;
    scopes.Peek().Add(name.lexeme, true);
  }

  private void ResolveLocal(Expr expr, Token name)
  {
    var enqueue = new Queue<Dictionary<string, bool>>();

    int depth = 0;
    while (scopes.Count != 0)
    {
      if (scopes.Peek().ContainsKey(name.lexeme))
      {
        interpreter.Resolve(expr, depth);
        break;
      }
      enqueue.Enqueue(scopes.Pop());
      depth++;
    }

    while (enqueue.Count != 0)
    {
      scopes.Push(enqueue.Dequeue());
    }
  }

}