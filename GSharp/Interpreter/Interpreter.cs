using System.Collections.Immutable;
using System.Data;
using System.Reflection;
using System.Text;
using GSharp.Exceptions;
using GSharp.Expression;
using GSharp.Objects;
using GSharp.Objects.Figures;
using GSharp.Parser;
using GSharp.Statement;
using static GSharp.TokenType;

namespace GSharp.Interpreter;

// <summary>
// Interpreter for GSharp code
// </summary>

public class Interpreter : IInterpreter<GSObject>, Expr.IVisitor<GSObject>, Stmt.IVisitor<VoidObject>
{
  private readonly Action<RuntimeError> runtimeErrorHandler;
  private readonly GSharpEnvironment globals = new();

  internal IBindingHandler bindingHandler { get; }

  private ImmutableList<Stmt> previousStmts = ImmutableList.Create<Stmt>();
  private IEnvironment<GSObject> currentEnvironment;

  public Interpreter(Action<RuntimeError> runtimeErrorHandler, IBindingHandler? bindingHandler = null)
  {
    this.runtimeErrorHandler = runtimeErrorHandler;
    this.bindingHandler = bindingHandler ?? new BindingHandler();

    this.currentEnvironment = globals;
  }

  public object? Eval(string source, ScanErrorHandler scanErrorHandler, ParseErrorHandler parseErrorHandler, NameResolutionErrorHandler nameResolutionErrorHandler)
  {
    if (System.String.IsNullOrWhiteSpace(source))
    {
      return null;
    }

    ScanAndParseResult result = Parser.Parser.ScanAndParse(source, scanErrorHandler, parseErrorHandler);

    if (result == ScanAndParseResult.scanErrorOccurred || result == ScanAndParseResult.parseErrorEncountered)
    {
      return null;
    }

    if (result.hasStatements)
    {
      var previousAndNewStmts = previousStmts.Concat(result.statements!).ToImmutableList();

      //
      // resolving names phase

      bool hasNameResolutionErrors = false;
      var nameResolver = new NameResolver(bindingHandler, nameResolutionError =>
      {
        hasNameResolutionErrors = true;
        nameResolutionErrorHandler(nameResolutionError);
      });

      nameResolver.Resolve(previousAndNewStmts);

      if (hasNameResolutionErrors)
      {
        return null;
      }

      //
      // type validation
      //

      bool typeValidationFailed = false;

      // ...

      if (typeValidationFailed)
      {
        return null;
      }

      previousStmts = previousAndNewStmts.ToImmutableList();

      try
      {
        Interpret(result.statements!);
      }
      catch (RuntimeError e)
      {
        runtimeErrorHandler(e);
        return VoidObject.Void;
      }

      return VoidObject.Void;
    }

    throw new IllegalStateException("syntax was not list of Stmt");
  }

  public string? Parse(string source, Action<ScanError> scanErrorHandler, Action<ParseError> parseErrorHandler)
  {
    // ... 
    // scanning phase
    // ...

    bool hasScanErrors = false;
    var scanner = new Scanner(source, scanError =>
    {
      hasScanErrors = true;
      scanErrorHandler(scanError);
    });

    var tokens = scanner.ScanTokens();

    if (hasScanErrors)
    {
      // something went wrong as early as the "scan" stage
      // abort the rest of the processing
      return null;
    }

    // ...
    // parsing phase
    // ...

    bool hasParseErrors = false;
    var parser = new Parser.Parser(tokens, parseError =>
    {
      hasParseErrors = true;
      parseErrorHandler(parseError);
    });

    object syntax = parser.ParseStmts();

    if (hasParseErrors)
    {
      // one or more parse error were encountered
      // they have been reported upstream
      // so we just abort the evaluation at this stage
      return null;
    }

    if (syntax is List<Stmt> stmts)
    {
      StringBuilder result = new();

      throw new NotImplementedException();
    }
    else
    {
      throw new IllegalStateException($"syntax expected to be List<Stmt>, not {syntax}");
    }
  }

  private void Interpret(IEnumerable<Stmt> statements)
  {
    foreach (var statement in statements)
    {
      try
      {
        Execute(statement);
      }
      catch (TargetInvocationException ex)
      {
        string message = ex.InnerException?.Message ?? ex.Message;

        throw new RuntimeError(null, message);
      }
      catch (SystemException ex)
      {
        throw new RuntimeError(null, ex.Message, ex);
      }
    }
  }

  public GSObject VisitLiteralExpr(Literal expr)
  {
    if (expr.Value is INumericLiteral parseNumber)
    {
      double value = (double)parseNumber.value;
      return new Objects.Scalar(value);
    }
    else if (expr.Value is bool parseBoolean)
    {
      return new Objects.Scalar(parseBoolean);
    }
    else if (expr.Value is string parseString)
    {
      return new Objects.String(parseString);
    }
    else if (expr.Value is null)
    {
      return new Objects.Undefined();
    }
    else
    {
      throw new ArgumentException($"Invalid literal found {expr}");
    }
  }

  public GSObject VisitLogicalExpr(Logical expr)
  {
    GSObject left = Evaluate(expr.Left)!;

    if (expr.Oper.type == OR)
    {
      if (left.GetTruthValue())
      {
        return new Objects.Scalar(true);
      }

      bool right = Evaluate(expr.Right)!.GetTruthValue();

      return new Scalar(right);
    }
    else if (expr.Oper.type == AND)
    {
      if (!left.GetTruthValue())
      {
        return new Objects.Scalar(false);
      }

      bool right = (bool)Evaluate(expr.Right)!.GetTruthValue();

      return new Scalar(right);
    }
    else
    {
      throw new RuntimeError(expr.Oper, $"Unsupported logical operator: {expr.Oper.type}");
    }
  }

  public GSObject VisitUnaryExpr(Unary expr)
  {
    GSObject right = Evaluate(expr.Right);

    switch (expr.Oper.type)
    {
      case NOT:
        return new Scalar(!right.GetTruthValue());
      case MINUS:
        return IOperate<Mult>.Operate(right, new Scalar(-1));
      default:
        throw new RuntimeError(expr.Oper, $"Unsupported operator encountered: {expr.Oper.type}");
    }
  }

  public GSObject VisitVariableExpr(Variable expr)
  {
    return LookUpVariable(expr.Name, expr);
  }

  private GSObject LookUpVariable(Token name, Variable identifier)
  {
    if (bindingHandler.GetLocalBinding(identifier, out Binding? localBinding))
    {
      if (localBinding is IDistanceBinding distanceBinding)
      {
        return currentEnvironment.GetAt(distanceBinding.Distance, name.lexeme);
      }
      else
      {
        throw new RuntimeError(name, $"Attempting to lookup variable for non-distance-aware biding '{localBinding}'");
      }
    }
    else
    {
      return globals.Get(name);
    }
  }

  public GSObject VisitGroupingExpr(Grouping expr)
  {
    return Evaluate(expr.Expression);
  }

  private GSObject Evaluate(Expr expr)
  {
    try
    {
      return expr.Accept(this);
    }
    catch (TargetInvocationException ex)
    {
      Token? token = (expr as IToken)?.Token;

      string message = ex.InnerException?.Message ?? ex.Message;

      throw new RuntimeError(token, message, ex);
    }
    catch (SystemException ex)
    {
      Token? token = (expr as IToken)?.Token;

      throw new RuntimeError(token, ex.Message, ex);
    }
  }

  private void Execute(Stmt stmt)
  {
    stmt.Accept(this);
  }

  public void ExecuteBlock(IEnumerable<Stmt> statements, IEnvironment<GSObject> blockEnvironment)
  {
    IEnvironment<GSObject> previousEnvironment = currentEnvironment;

    try
    {
      currentEnvironment = blockEnvironment;

      foreach (var statement in statements)
      {
        Execute(statement);
      }
    }
    finally
    {
      currentEnvironment = previousEnvironment;
    }
  }

  public GSObject VisitBinaryExpr(Binary expr)
  {
    GSObject left = Evaluate(expr.Left);
    GSObject right = Evaluate(expr.Right);

    switch (expr.Oper.type)
    {
      case GREATER:
        bool IsGreater = !(IOperate<LessTh>.Operate(left, right).GetTruthValue() || left.Equals(right));
        return new Scalar(IsGreater);
      case GREATER_EQUAL:
        bool IsGreaterOrEqual = !IOperate<LessTh>.Operate(left, right).GetTruthValue();
        return new Scalar(IsGreaterOrEqual);
      case LESS:
        return IOperate<LessTh>.Operate(left, right);
      case LESS_EQUAL:
        bool IsLessOrEqual = IOperate<LessTh>.Operate(left, right).GetTruthValue() || left.Equals(right);
        return new Scalar(IsLessOrEqual);
      case NOT_EQUAL:
        return new Scalar(!left.Equals(right));
      case EQUAL_EQUAL:
        return new Scalar(left.Equals(right));
      case PLUS:
        return IOperate<Add>.Operate(left, right);
      case MINUS:
        return IOperate<Subst>.Operate(left, right);
      case MUL:
        return IOperate<Mult>.Operate(left, right);
      case DIV:
        return IOperate<Div>.Operate(left, right);
      case MOD:
        return IOperate<Mod>.Operate(left, right);
      case POWER:
        throw new NotImplementedException();
      default:
        string message = InterpreterMessages.UnsupportedOperandTypes(expr.Oper.type, left, right);
        throw new RuntimeError(expr.Oper, message);
    }
  }

}