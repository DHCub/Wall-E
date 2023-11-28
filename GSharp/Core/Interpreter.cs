namespace GSharp.Core;

using System.Collections.Generic;
using GSharp.Expression;
using GSharp.Statement;
using GSharp.Collections;
using System;
using static TokenType;

public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
  public readonly GSharp.Environment globals;
  private GSharp.Environment environment;
  private readonly Dictionary<Expr, int> locals;
  private readonly ILogger logger;

  public Interpreter(ILogger logger)
  {
    this.logger = logger;
    this.globals = new GSharp.Environment();
    this.environment = globals;
    this.locals = new Dictionary<Expr, int>();
  }

  public void Interpret(List<Stmt> statements)
  {
    try
    {
      foreach (var statement in statements)
      {
        Execute(statement);
      }
    }
    catch (RuntimeError error)
    {
      logger.RuntimeError(error);
    }
  }

  private object Evaluate(Expr expr)
  {
    return expr.Accept(this);
  }

  private void Execute(Stmt stmt)
  {
    stmt.Accept(this);
  }

  public void Resolve(Expr expr, int depth)
  {
    locals.Add(expr, depth);
  }

  public void ExecuteBlock(List<Stmt> statements, GSharp.Environment environment)
  {
    GSharp.Environment previous = this.environment;
    try
    {
      this.environment = environment;
      foreach (var statement in statements)
      {
        Execute(statement);
      }
    }
    finally
    {
      this.environment = previous;
    }
  }

  public object VisitBinaryExpr(Binary expr)
  {
    object left = Evaluate(expr.left);
    object right = Evaluate(expr.right);

    switch (expr.oper.type)
    {
      case NOT_EQUAL:
        return !IsEqual(left, right);
      case EQUAL_EQUAL:
        return IsEqual(left, right);
      case GREATER:
        CheckNumberOperands(expr.oper, left, right);
        return (double)left > (double)right;
      case GREATER_EQUAL:
        CheckNumberOperands(expr.oper, left, right);
        return (double)left >= (double)right;
      case LESS:
        CheckNumberOperands(expr.oper, left, right);
        return (double)left < (double)right;
      case LESS_EQUAL:
        CheckNumberOperands(expr.oper, left, right);
        return (double)left <= (double)right;
      case MINUS:
        CheckNumberOperands(expr.oper, left, right);
        return (double)left - (double)right;
      case PLUS:
        if (left is double && right is double)
        {
          return (double)left + (double)right;
        }

        // sum of sequences
        throw new NotImplementedException();

      // logger.Error("RTE", expr.oper, "Operands must be two numbers or two sequences.");
      case DIV:
        CheckNumberOperands(expr.oper, left, right);
        return (double)left / (double)right;
      case MUL:
        CheckNumberOperands(expr.oper, left, right);
        return (double)left * (double)right;
    }

    return null;
  }

  public object VisitCallExpr(Call expr)
  {
    object calle = Evaluate(expr.calle);

    List<object> arguments = new List<object>();
    foreach (var argument in expr.arguments)
    {
      arguments.Add(Evaluate(argument));
    }

    if (calle is not ICallable)
    {
      logger.Error("RTE", expr.paren, "Can only call functions.");
      return null;
    }

    ICallable function = (ICallable)calle;

    if (arguments.Count != function.Arity())
    {
      logger.Error("RTE", expr.paren, $"Expected {function.Arity} arguments but got {arguments.Count}.");
    }

    return function.Call(this, arguments);
  }

  public object VisitConditionalExpr(Conditional expr)
  {
    if (IsTruthy(Evaluate(expr.condition)))
    {
      return Evaluate(expr.thenBranch);
    }
    return Evaluate(expr.elseBranch);
  }

  public object VisitGroupingExpr(Grouping expr)
  {
    return Evaluate(expr.expression);
  }

  public object VisitIntRangeExpr(IntRange expr)
  {
    if (expr.right is null)
    {
      CheckIntegerOperand(expr.dots, expr.left.literal);
      return InfiniteSequence((int)expr.left.literal);
    }
    CheckIntegerOperands(expr.dots, expr.left.literal, expr.right.literal);
    return FiniteSequence((int)expr.left.literal, (int)expr.right.literal);
  }

  public object VisitLetInExpr(LetIn expr)
  {
    ExecuteBlock(expr.instructions, new GSharp.Environment(environment));
    return Evaluate(expr.body);
  }

  public object VisitLiteralExpr(Literal expr)
  {
    return expr.value;
  }

  public object VisitLogicalExpr(Logical expr)
  {
    object left = Evaluate(expr.left);

    if (expr.oper.type == OR)
    {
      if (IsTruthy(left)) return left;
    }
    else
    {
      if (!IsTruthy(left)) return left;
    }

    return Evaluate(expr.right);
  }

  public object VisitSequenceExpr(Sequence expr)
  {
    throw new NotImplementedException();
  }

  public object VisitUnaryExpr(Unary expr)
  {
    object right = Evaluate(expr.right);

    switch (expr.oper.type)
    {
      case NOT:
        return !IsTruthy(right);
      case MINUS:
        CheckNumberOperand(expr.oper, right);
        return -(double)right;
    }

    return null;
  }

  public object VisitUndefinedExpr(Undefined expr)
  {
    return null;
  }

  public object VisitVariableExpr(Variable expr)
  {
    return LookUpVariable(expr.name, expr);
  }

  public object VisitColorStmt(Color stmt)
  {
    PenChangeColor(stmt.color);
    return null;
  }

  public object VisitConstantStmt(Constant stmt)
  {
    throw new NotImplementedException();
  }

  public object VisitDrawStmt(Draw stmt)
  {
    DrawElement(stmt.elements);
    return null;
  }

  public object VisitExpressionStmt(Expression stmt)
  {
    Evaluate(stmt.expression);
    return null;
  }

  public object VisitFunctionStmt(Function stmt)
  {
    GSFunction function = new GSFunction(stmt, environment, false);
    environment.Define(stmt.name.lexeme, function);
    return null;
  }

  public object VisitImportStmt(Import stmt)
  {
    throw new NotImplementedException();
  }

  public object VisitPrintStmt(Print stmt)
  {
    foreach (var expr in stmt.printe)
    {
      object value = Evaluate(expr);
      Console.WriteLine(Stringify(value));
    }
    return null;
  }

  public object VisitRestoreStmt(Restore stmt)
  {
    PenRestoreColor();
    return null;
  }

  public object VisitVarStmt(Var stmt)
  {
    object value = null;
    if (stmt.initializer is not null)
    {
      value = Evaluate(stmt.initializer);
    }
    environment.Define(stmt.name.lexeme, value);
    return null;
  }

  private void DrawElement(object element)
  {
    System.Console.WriteLine("Dibuja un elemento");
  }

  private void PenChangeColor(Token color)
  {
    System.Console.WriteLine("Cambia el color del pincel a" + color.ToString());
  }

  private void PenRestoreColor()
  {
    System.Console.WriteLine("Restaura color del pincel");
  }

  private object InfiniteSequence(int start)
  {
    throw new NotImplementedException();
  }

  private object FiniteSequence(int start, int end)
  {
    throw new NotImplementedException();
  }

  private object LookUpVariable(Token name, Expr expr)
  {
    if (locals.ContainsKey(expr))
    {
      int distance = locals[expr];
      return environment.GetAt(distance, name.lexeme);
    }
    else
    {
      return globals.Get(name);
    }
  }

  private void CheckNumberOperand(Token oper, object operand)
  {
    if (operand is double) return;
    logger.Error("RTE", oper, "Operand must be a number.");
  }

  private void CheckNumberOperands(Token oper, object left, object right)
  {
    if (left is double && right is double) return;
    logger.Error("RTE", oper, "Operands must be numbers.");
  }

  private void CheckIntegerOperand(Token oper, object operand)
  {
    if (operand is double)
    {
      if (double.IsInteger((double)operand))
      {
        return;
      }
    }
    logger.Error("RTE", oper, "Operand must be a integer.");
  }

  private void CheckIntegerOperands(Token oper, object left, object right)
  {
    if (left is double && right is double)
    {
      if (double.IsInteger((double)left) && double.IsInteger((double)right))
      {
        return;
      }
    }
    logger.Error("RTE", oper, "Operands must be integers.");
  }

  private bool IsTruthy(object obj)
  {
    if (obj is null) return false;
    if (obj is bool) return (bool)obj;
    return true;
  }

  private bool IsEqual(object a, object b)
  {
    if (a is null && b is null) return true;
    if (a is null) return false;
    return a.Equals(b);
  }

  private string Stringify(object obj)
  {
    if (obj == null) return "null";

    if (obj is double)
    {
      string text = obj.ToString();
      if (text.EndsWith(".0"))
      {
        text = text.Substring(0, text.Length - 2);
      }
      return text;
    }

    return obj.ToString();
  }
}