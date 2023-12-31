using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using GSharp.Exceptions;
using GSharp.Expression;
using GSharp.Objects;
using GSharp.Objects.Collections;
using GSharp.Objects.Figures;
using GSharp.Parser;
using GSharp.Statement;
using static GSharp.TokenType;
using GSharp.GUIInterface;
using GSharp;
using System.Net.Mail;
using GSharp.Types;

namespace GSharp.Interpreter;

/// <summary>
/// Interpreter for GSharp code
/// </summary>
public class Interpreter : IInterpreter, Expr.IVisitor<GSObject>, Stmt.IVisitor<VoidObject>
{
  private readonly Action<RuntimeError> runtimeErrorHandler;
  private readonly GSharpEnvironment globals = new();

  internal IBindingHandler BindingHandler { get; }

  private readonly Action<string> standardOutputHandler;

  private ImmutableList<Stmt> previousStmts = ImmutableList.Create<Stmt>();
  private IEnvironment currentEnvironment;

  private Action<Colors, Figure> drawFigure;
  private Action<Colors, Figure, string> drawLabeledFigure;
  private readonly Func<string, string> importHandler;

  private const int MaxNumberOfCalls = 2000;
  private int NumberOfCalls = 0;

  private HashSet<string> builtins = new HashSet<string>()
  {
    "line",
    "segment",
    "ray",
    "arc",
    "circle",
    "measure",
    "intersect",
    "count",
    "randoms",
    "points",
    "samples",
    "point"
  };

  public Func<string, Stack<string>, List<Stmt>> newImportHandler;
  private readonly Stack<Colors> colors;

  private Stack<string> importTrace;
  private Stack<string> stackTrace;
  private List<string> importedFiles;

  public Interpreter(Action<RuntimeError> runtimeErrorHandler, Action<string> standardOutputHandler, Func<string, string> importHandler, Action<Colors, Figure> drawFigure, Action<Colors, Figure, string> drawLabeledFigure, IBindingHandler? bindingHandler = null)
  {
    this.runtimeErrorHandler = runtimeErrorHandler;
    this.BindingHandler = bindingHandler ?? new BindingHandler();
    this.standardOutputHandler = standardOutputHandler;
    this.drawFigure = drawFigure;
    this.drawLabeledFigure = drawLabeledFigure;
    this.importHandler = importHandler;
    this.importTrace = new();
    this.stackTrace = new();
    this.importedFiles = new();

    colors = new();

    colors.Push(Colors.Black);

    this.currentEnvironment = globals;
  }

  public object? Eval(string source, ScanErrorHandler scanErrorHandler, ParseErrorHandler parseErrorHandler, NameResolutionErrorHandler nameResolutionErrorHandler, SemanticErrorHandler semanticErrorHandler)
  {
    if (string.IsNullOrWhiteSpace(source))
    {
      return null;
    }

    ScanAndParseResult result = Parser.Parser.ScanAndParse(source, scanErrorHandler, parseErrorHandler, importTrace);

    if (result == ScanAndParseResult.scanErrorOccurred || result == ScanAndParseResult.parseErrorEncountered)
    {
      return null;
    }

    if (result.hasStatements)
    {
      var previousAndNewStmts = previousStmts.Concat(result.statements!).ToImmutableList();

      //
      // resolving names phase
      //
      bool hasNameResolutionErrors = false;
      var nameResolver = new NameResolver(BindingHandler, nameResolutionError =>
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

      List<Stmt> newImportHandler2(string dir, Stack<string> importTrace)
      {
        var src = importHandler(dir);
        if (src == null) return null;
        var liststmt = Parse(src, scanErrorHandler, parseErrorHandler, importTrace);
        if (liststmt is null)
        {
          throw new RuntimeError(null, null, null);
        }
        return liststmt;
      }

      this.newImportHandler = newImportHandler2;

      var semanticAnalyzer = new SemanticAnalyzer(result.statements, semanticAnalizerError =>
      {
        typeValidationFailed = true;
        semanticErrorHandler(semanticAnalizerError);
      }, newImportHandler);

      try { semanticAnalyzer.Analyze(); }
      catch (RuntimeError) { return null; }

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
        // standardOutputHandler(e.StackTrace);
        return VoidObject.Void;
      }

      return VoidObject.Void;
    }

    throw new IllegalStateException("syntax was not list of Stmt", importTrace);
  }

  public VoidObject ResolveAndInterpret(List<Stmt> parseResult, NameResolutionErrorHandler nameResolutionErrorHandler)
  {
    var previousAndNewStmts = previousStmts.Concat(parseResult!).ToImmutableList();

    //
    // resolving names phase
    //

    bool hasNameResolutionErrors = false;
    var nameResolver = new NameResolver(BindingHandler, nameResolutionError =>
    {
      hasNameResolutionErrors = true;
      nameResolutionErrorHandler(nameResolutionError);
    });

    nameResolver.Resolve(previousAndNewStmts);

    if (hasNameResolutionErrors)
    {
      return null;
    }

    previousStmts = previousAndNewStmts.ToImmutableList();

    try
    {
      Interpret(parseResult);
    }
    catch (RuntimeError e)
    {
      runtimeErrorHandler(e);
      return VoidObject.Void;
    }

    return VoidObject.Void;
  }

  public List<Stmt> Parse(string source, ScanErrorHandler scanErrorHandler, ParseErrorHandler parseErrorHandler, Stack<string> importTrace)
  {
    // ... 
    // scanning phase
    // ...

    bool hasScanErrors = false;
    var scanner = new Scanner(source, scanError =>
    {
      hasScanErrors = true;
      scanErrorHandler(scanError);
    }, importTrace);

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
    var parser = new GSharp.Parser.Parser(tokens, parseError =>
    {
      hasParseErrors = true;
      parseErrorHandler(parseError);
    }, importTrace);

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
      return stmts;
    }
    else
    {
      throw new IllegalStateException($"syntax expected to be List<Stmt>, not {syntax}", importTrace);
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

        throw new RuntimeError(null, message, stackTrace);
      }
      catch (StackOverflowException ex)
      {
        throw new RuntimeError(null, ex.Message, stackTrace, ex);
      }
      catch (SystemException ex)
      {
        throw new RuntimeError(null, ex.Message, stackTrace, ex);
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
      throw new RuntimeError(expr.Oper, $"Unsupported logical operator: {expr.Oper.type}", stackTrace);
    }
  }

  public GSObject VisitAssignExpr(Assign expr)
  {
    GSObject value = Evaluate(expr.Value);

    try
    {
      if (BindingHandler.GetLocalBinding(expr, out Binding? binding))
      {
        if (binding is IDistanceBinding distanceBinding)
        {
          GSObject curValue = currentEnvironment.GetAt(distanceBinding.Distance, expr.Name.lexeme);

          if (!curValue.SameTypeAs(value))
          {
            throw new RuntimeError(expr.Name, $"Expected value of type {curValue.GetTypeName()}", stackTrace);
          }

          currentEnvironment.AssignAt(distanceBinding.Distance, expr.Name, value);
        }
        else
        {
          throw new RuntimeError(expr.Name, $"Unsupported variable assignment encountered for non-distance-aware binding '{binding}'.", stackTrace);
        }
      }
      else
      {
        GSObject curValue = globals.Get(expr.Name);

        if (!curValue.SameTypeAs(value))
        {
          throw new RuntimeError(expr.Name, $"Expected value of type {curValue.GetTypeName()}", stackTrace);
        }

        globals.Assign(expr.Name, value);
      }

      return value;
    }
    catch (RuntimeError e)
    {
      e.AddStackTrace(stackTrace);
      throw e;
    }
  }

  public GSObject VisitUnaryPrefixExpr(UnaryPrefix expr)
  {
    GSObject right = Evaluate(expr.Right);

    try
    {
      switch (expr.Oper.type)
      {
        case NOT:
          return new Scalar(!right.GetTruthValue());
        case MINUS:
          return IOperate<Mult>.Operate(right, new Scalar(-1));
        default:
          throw new RuntimeError(expr.Oper, $"Unsupported operator encountered: {expr.Oper.type}", null);
      }
    }
    catch (RuntimeError e)
    {
      e.AddStackTrace(stackTrace);
      throw e;
    }
  }

  public GSObject VisitUnaryPostfixExpr(UnaryPostfix expr)
  {
    GSObject left = Evaluate(expr.Left);

    try
    {
      if (!left.SameTypeAs(new Scalar(0)))
      {
        switch (expr.Oper.type)
        {
          case PLUS_PLUS:
            throw new RuntimeError(expr.Oper, $"++ can only be used to increment scalars, not {left.GetTypeName()}", stackTrace);
          case MINUS_MINUS:
            throw new RuntimeError(expr.Oper, $"-- can only be used to decrement scalars, not {left.GetTypeName()}", stackTrace);
          default:
            throw new RuntimeError(expr.Oper, $"Unsupported operator encountered: {expr.Oper.type}", stackTrace);
        }
      }

      var variable = (Variable)expr.Left;

      GSObject value;
      switch (left)
      {
        case Scalar prevScalarValue:
          value = expr.Oper.type switch
          {
            PLUS_PLUS => IOperate<Add>.Operate(prevScalarValue, new Scalar(1)),
            MINUS_MINUS => IOperate<Subst>.Operate(prevScalarValue, new Scalar(1)),
            _ => throw new RuntimeError(expr.Oper, $"Unsupported operator encountered: {expr.Oper.type}", stackTrace)
          };
          break;
        default:
          throw new RuntimeError(expr.Oper, $"Internal runtime: Unsupported type {left} encountered with {expr.Oper.type} operator.", stackTrace);
      }

      if (BindingHandler.GetLocalBinding(expr, out Binding? binding))
      {
        if (binding is IDistanceBinding distanceBinding)
        {
          currentEnvironment.AssignAt(distanceBinding.Distance, expr.Name, value);
        }
        else
        {
          throw new RuntimeError(expr.Oper, $"Unsupported operator '{expr.Oper.type}' encountered for non-distance-aware binding '{binding}'.", stackTrace);
        }
      }
      else
      {
        globals.Assign(variable.Name, value);
      }

      return left!;
    }
    catch (RuntimeError e)
    {
      e.AddStackTrace(stackTrace);
      throw e;
    }
  }

  public GSObject VisitVariableExpr(Variable expr)
  {
    return LookUpVariable(expr.Name, expr);
  }

  private GSObject LookUpVariable(Token name, Variable identifier)
  {
    if (BindingHandler.GetLocalBinding(identifier, out Binding? localBinding))
    {
      if (localBinding is IDistanceBinding distanceBinding)
      {
        return currentEnvironment.GetAt(distanceBinding.Distance, name.lexeme);
      }
      else
      {
        throw new RuntimeError(name, $"Attempting to lookup variable for non-distance-aware biding '{localBinding}'", stackTrace);
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

  public GSObject VisitIndexExpr(Index expr)
  {
    GSObject indexee = Evaluate(expr.Indexee);


    try
    {
      // if (indexee.SameTypeAs(new Objects.Undefined()))
      // {
      //   throw new RuntimeError(expr.ClosingBracket, $"'undefined' reference cannot be indexed.", stackTrace);
      // }

      // GSObject argument = Evaluate(expr.Argument);

      // if (!argument.SameTypeAs(new Scalar(0)))
      // {
        // throw new RuntimeError(expr.ClosingBracket, $"Cannot index by '{argument.GetTypeName()}' key.", stackTrace);
      // }

      // Scalar index = (Scalar)argument;

      // if (indexee is Objects.Collections.Sequence seq)
      // {
      //   return seq[(int)index.value];
      // }
      // else if (indexee is Objects.String str)
      // {
      //   return new Objects.String(str.value[(int)index.value].ToString());
      // }
      // else
      // {
        // throw new RuntimeError(expr.ClosingBracket, $"Indexing {indexee.GetTypeName()} by {argument.GetTypeName()} is not supported.", stackTrace);
      // }

      GSObject argument = Evaluate(expr.Argument);
      return IOperate<Indexer>.Operate(indexee, argument);
    }
    catch (RuntimeError e)
    {
      e.AddStackTrace(stackTrace);
      throw e;
    }
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

      throw new RuntimeError(token, message, stackTrace, ex);
    }
    catch (SystemException ex)
    {
      Token? token = (expr as IToken)?.Token;
      throw new RuntimeError(token, ex.Message, stackTrace, ex);
    }
  }

  private void Execute(Stmt stmt)
  {
    stmt.Accept(this);
  }

  public void ExecuteBlock(IEnumerable<Stmt>? statements, IEnvironment blockEnvironment)
  {
    IEnvironment previousEnvironment = currentEnvironment;

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

    try
    {
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
        case PLUS_EQUAL:
          return IOperate<Add>.Operate(left, right);
        case MINUS_EQUAL:
          return IOperate<Subst>.Operate(left, right);
        case MUL:
          return IOperate<Mult>.Operate(left, right);
        case DIV:
          return IOperate<Div>.Operate(left, right);
        case MOD:
          return IOperate<Mod>.Operate(left, right);
        case POWER:
          return IOperate<Power>.Operate(left, right);
        default:
          string message = InterpreterMessages.UnsupportedOperandTypes(expr.Oper.type, left, right);
          throw new RuntimeError(expr.Oper, message, null);
      }
    }
    catch (RuntimeError e)
    {
      e.AddStackTrace(stackTrace);
      throw e;
    }
  }

  public GSObject VisitCallExpr(Call expr)
  {
    if (NumberOfCalls >= MaxNumberOfCalls)
    {
      throw new StackOverflowException();
    }

    GSObject calle = Evaluate(expr.Calle);

    var arguments = new List<GSObject>();
    foreach (var argument in expr.Arguments)
    {

      arguments.Add(Evaluate(argument));
    }

    switch (calle)
    {
      case ICallable callable:
        if (arguments.Count != callable.Arity())
        {
          throw new RuntimeError(expr.Paren, "Expected" + callable.Arity() + " argument(s) but got " + arguments.Count + ".", stackTrace);
        }

        NumberOfCalls++;
        stackTrace.Push(expr.TokenAwareCalle.Token.lexeme);

        try
        {
          var returnValue = callable.Call(this, arguments);
          NumberOfCalls--;
          stackTrace.Pop();

          return returnValue;
        }
        catch (RuntimeError)
        {
          throw;
        }
        catch (Exception e)
        {
          if (expr.Calle is Variable variable)
          {
            throw new RuntimeError(variable.Name, $"{variable.Name.lexeme}: {e.Message}", stackTrace);
          }
          else
          {
            throw new RuntimeError(expr.Paren, e.Message, stackTrace);
          }
        }
      default:
        if (expr.Calle is Variable funName)
        {
          if (builtins.Contains(funName.Name.lexeme))
          {
            return BuiltinHandler(funName.Name.lexeme, arguments, expr);
          }
        }
        throw new RuntimeError(expr.Paren, $"Can only call functions and native methods, not {calle}", stackTrace);
    }
  }

  private GSObject BuiltinHandler(string funName, List<GSObject> arguments, Call expr)
  {
    switch (funName)
    {
      case "point":
        if (arguments[0].SameTypeAs(new Scalar(0)) && arguments[1].SameTypeAs(new Scalar(0)))
        {
          return new Point(((Scalar)arguments[0]).value, ((Scalar)arguments[1]).value);
        }
        throw new RuntimeError(expr.Token, "Expected scalars as arguments.", stackTrace);
      case "line":
        if (arguments[0].SameTypeAs(new Point()) && arguments[1].SameTypeAs(new Point()))
        {
          return new Line((Point)arguments[0], (Point)arguments[1]);
        }
        throw new RuntimeError(expr.Token, "Expected point as arguments.", stackTrace);
      case "segment":
        if (arguments[0].SameTypeAs(new Point()) && arguments[1].SameTypeAs(new Point()))
        {
          return new Segment((Point)arguments[0], (Point)arguments[1]);
        }
        throw new RuntimeError(expr.Token, "Expected point as arguments.", stackTrace);
      case "ray":
        if (arguments[0].SameTypeAs(new Point()) && arguments[1].SameTypeAs(new Point()))
        {
          return new Ray((Point)arguments[0], (Point)arguments[1]);
        }
        throw new RuntimeError(expr.Token, "Expected point as arguments.", stackTrace);
      case "arc":
        if (arguments[0].SameTypeAs(new Point()) && arguments[1].SameTypeAs(new Point()) && arguments[2].SameTypeAs(new Point()) && arguments[3].SameTypeAs(new Measure(0.0)))
        {
          return new Arc((Point)arguments[0], (Point)arguments[1], (Point)arguments[2], ((Measure)arguments[3]).value);
        }
        throw new RuntimeError(expr.Token, "Invalid arguments.", stackTrace);
      case "circle":
        if (arguments[0].SameTypeAs(new Point()) && arguments[1].SameTypeAs(new Measure(0.0)))
        {
          return new Circle((Point)arguments[0], ((Measure)arguments[1]).value);
        }
        throw new RuntimeError(expr.Token, "Expected point and measure as arguments.", stackTrace);
      case "measure":
        if (arguments[0].SameTypeAs(new Point()) && arguments[1].SameTypeAs(new Point()))
        {
          return new Measure(((Point)arguments[0]).DistanceTo((Point)arguments[1]));
        }
        throw new RuntimeError(expr.Token, "Expected point as arguments.", stackTrace);
      case "intersect":
        if (arguments[0] is Figure fig1 && arguments[1] is Figure fig2)
        {
          return Functions.Intersect(fig1, fig2);
        }
        throw new RuntimeError(expr.Token, "Expected figures as arguments.", stackTrace);
      case "count":
        if (arguments[0] is Objects.Collections.Sequence seq)
        {
          return seq.GSCount();
        }
        throw new RuntimeError(expr.Token, "Expected sequence as argument.", stackTrace);
      case "randoms":
        return new GeneratorSequence(new RandomDoubleGenerator());
      case "points":
        if (arguments[0] is Figure fig)
        {
          return new GeneratorSequence(new RandomPointInFigureGenerator(fig));
        }
        throw new RuntimeError(expr.Token, "Expected figure as argument.", stackTrace);
      case "samples":
        return new GeneratorSequence(new RandomPointInCanvasGenerator());
      default:
        throw new Exception($"Not found {funName}");
    }
  }

  public GSObject VisitConditionalExpr(Conditional expr)
  {
    bool condition = Evaluate(expr.Condition)!.GetTruthValue();

    if (condition)
    {
      return Evaluate(expr.ThenBranch);
    }
    else
    {
      return Evaluate(expr.ElseBranch);
    }
  }

  public GSObject VisitEmptyExpr(Empty expr)
  {
    return new Objects.Undefined();
  }

  public GSObject VisitIntRangeExpr(IntRange expr)
  {
    if (expr.Right is null)
    {
      int left = int.Parse(expr.Left.lexeme);
      if (expr.LeftNegative) left = -left;

      return new GeneratorSequence(new IntRangeGenerator(left));
    }
    else
    {
      List<GSObject> rangeInt = new();
      int left = int.Parse(expr.Left.lexeme);
      int right = int.Parse(expr.Right.lexeme);

      if (expr.LeftNegative) left = -left;
      if (expr.RightNegative) right = -right;

      if (left > right)
      {
        throw new RuntimeError(expr.Token, $"Invalid range: {left} expected to be less or equal to {right}", stackTrace);
      }

      for (int i = left; i <= right; i++)
      {
        rangeInt.Add(new Scalar(i));
      }

      return new FiniteStaticSequence(rangeInt);
    }
  }

  public GSObject VisitLetInExpr(LetIn expr)
  {
    var LetEnvironment = new GSharpEnvironment(currentEnvironment);

    try
    {
      ExecuteBlock(expr.Stmts, LetEnvironment);
      return new Objects.Undefined();
    }
    catch (Exceptions.Return returnValue)
    {
      return returnValue.Value;
    }
  }

  public GSObject VisitSequenceExpr(Expression.Sequence expr)
  {
    if (expr.Items.Count == 0)
    {
      return new FiniteStaticSequence();
    }

    bool IsFinite = true;
    GeneratorSequence genSequence = null;
    GSObject firstSeqValue = null;

    List<GSObject> prefix = new();
    for (int i = 0; i < expr.Items.Count; i++)
    {
      if (IsFinite)
      {
        if (expr.Items[i] is IntRange range)
        {
          if (i == 0)
          {
            firstSeqValue = new Scalar(0);
          }
          else
          {
            if (!firstSeqValue.SameTypeAs(new Scalar(0)))
            {
              throw new RuntimeError(expr.Token, "Sequences values should be of the same type", stackTrace);
            }
          }

          if (range.Right is null)
          {
            IsFinite = false;
            int left = int.Parse(range.Left.lexeme);

            if (range.LeftNegative) left = -left;

            genSequence = new GeneratorSequence(new IntRangeGenerator(left), prefix);
          }
          else
          {
            int left = int.Parse(range.Left.lexeme);
            int right = int.Parse(range.Right.lexeme);

            if (range.LeftNegative) left = -left;
            if (range.RightNegative) right = -right;

            if (left > right)
            {
              throw new RuntimeError(expr.Token, $"Invalid range: {left} expected to be less or equal to {right}", stackTrace);
            }

            for (int v = left; v <= right; v++)
            {
              prefix.Add(new Scalar(v));
            }
          }
        }
        else
        {
          GSObject curVal = Evaluate(expr.Items[i]);

          if (i == 0)
          {
            firstSeqValue = curVal;
          }
          else
          {
            if (!firstSeqValue.SameTypeAs(curVal))
            {
              throw new RuntimeError(expr.Token, "Sequences values should be of the same type", stackTrace);
            }
          }

          prefix.Add(curVal);
        }
      }
      else
      {
        GSObject curVal = Evaluate(expr.Items[i]);

        if (i == 0)
        {
          firstSeqValue = curVal;
        }
        else
        {
          if (!firstSeqValue.SameTypeAs(curVal))
          {
            throw new RuntimeError(expr.Token, "Sequences values should be of the same type", stackTrace);
          }
        }
      }
    }

    if (IsFinite)
    {
      return new FiniteStaticSequence(prefix);
    }

    return genSequence;
  }

  public GSObject VisitUndefinedExpr(Expression.Undefined expr)
  {
    return new Objects.Undefined();
  }

  public VoidObject VisitBlockStmt(Block stmt)
  {
    ExecuteBlock(stmt.Statements, new GSharpEnvironment(currentEnvironment));
    return VoidObject.Void;
  }

  public VoidObject VisitWhileStmt(While stmt)
  {
    while (Evaluate(stmt.Condition)!.GetTruthValue())
    {
      Execute(stmt.Body);
    }

    return VoidObject.Void;
  }

  public VoidObject VisitColorStmt(ColorStmt stmt)
  {
    colors.Push(GUIInterface.GUIInterface.GetColor(stmt.Color.lexeme));
    return VoidObject.Void;
  }

  public VoidObject VisitConstantStmt(ConstantStmt stmt)
  {
    GSObject value = Evaluate(stmt.Initializer);
    if (value is Objects.Collections.Sequence valueSeq)
    {
      int cntConsts = stmt.Names.Count;
      for (int i = 0; i < cntConsts - 1; i++)
      {
        try { currentEnvironment.Define(stmt.Names[i], valueSeq[i]); }
        catch (RuntimeError e) { e.AddStackTrace(stackTrace); throw e; }
      }

      try { currentEnvironment.Define(stmt.Names.Last(), valueSeq.GetRemainder(cntConsts - 1)); }
      catch (RuntimeError e) { e.AddStackTrace(stackTrace); throw e; }
    }
    else
    {
      if (stmt.Names.Count != 1)
      {
        throw new RuntimeError(stmt.Token, "Cannot assign some constants to unique value.", stackTrace);
      }

      try { currentEnvironment.Define(stmt.Names[0], value); }
      catch (RuntimeError e) { e.AddStackTrace(stackTrace); throw e; }
    }

    return VoidObject.Void;
  }

  public VoidObject VisitDrawStmt(Draw stmt)
  {
    var drawe = Evaluate(stmt.Elements);

    void draw(GSObject gso)
    {
      if (gso is FiniteStaticSequence finSeq)
      {
        foreach (var elem in finSeq)
        {
          draw(elem);
        }
      }
      else if (gso is Figure figure)
      {
        if (stmt.Label == null)
        {
          drawFigure(colors.Peek(), figure);
        }
        else drawLabeledFigure(colors.Peek(), figure, (string)stmt.Label.literal);
      }
      else if (gso is GSharp.Objects.Collections.Sequence)
      {
        throw new RuntimeError(stmt.Command, $"Cannot draw Infinite Sequence", stackTrace);
      }

      else throw new RuntimeError(stmt.Command, $"Cannot draw {gso.GetTypeName()}", stackTrace);
    }

    draw(drawe);

    return VoidObject.Void;
  }

  public VoidObject VisitExpressionStmt(ExpressionStmt stmt)
  {
    Evaluate(stmt.Expression);
    return VoidObject.Void;
  }

  public VoidObject VisitFunctionStmt(Function stmt)
  {
    var function = new GSFunction(stmt, currentEnvironment, importTrace);
    try { currentEnvironment.Define(stmt.Name, function); }
    catch (RuntimeError e) { e.AddStackTrace(stackTrace); throw e; }
    return VoidObject.Void;
  }

  public VoidObject VisitImportStmt(Import stmt)
  {
    var dir = (string)stmt.DirName.literal;
    if (importedFiles.Contains(dir)) return VoidObject.Void;
    importTrace.Push(dir);
    importedFiles.Add(dir);
    ResolveAndInterpret(newImportHandler(dir, importTrace), Console.WriteLine);
    importTrace.Pop();

    return VoidObject.Void;
  }

  public VoidObject VisitPrintStmt(Print stmt)
  {
    GSObject? value = Evaluate(stmt.Expression);
    if (stmt.Label is not null)
    {
      standardOutputHandler(stmt.Label.literal + ": " + value.ToString());
    }
    else
    {
      standardOutputHandler(value.ToString());
    }

    return VoidObject.Void;
  }

  public VoidObject VisitRestoreStmt(Restore stmt)
  {
    if (colors.Count > 1) colors.Pop();

    return VoidObject.Void;
  }

  public VoidObject VisitReturnStmt(Statement.Return stmt)
  {
    GSObject value = new Objects.Undefined();
    if (stmt.Value is not null)
    {
      value = Evaluate(stmt.Value);
    }

    throw new Exceptions.Return(value);
  }

  public VoidObject VisitVarStmt(Var stmt)
  {
    GSObject value;

    if (stmt.Initializer is not null)
    {
      value = Evaluate(stmt.Initializer);
    }
    else
    {
      switch (stmt.TypeReference.TypeSpecifier.type)
      {
        case POINT:
          value = new Point();
          break;
        case LINE:
          value = new Line();
          break;
        case SEGMENT:
          value = new Segment();
          break;
        case RAY:
          value = new Ray();
          break;
        case CIRCLE:
          value = new Circle();
          break;
        case ARC:
          value = new Arc();
          break;
        case POINT_SEQUENCE:
          value = new GeneratorSequence(new RandomFigureGenerator(FigureOptions.Point));
          break;
        case LINE_SEQUENCE:
          value = new GeneratorSequence(new RandomFigureGenerator(FigureOptions.Line));
          break;
        case CIRCLE_SEQUENCE:
          value = new GeneratorSequence(new RandomFigureGenerator(FigureOptions.Circle));
          break;
        case RAY_SEQUENCE:
          value = new GeneratorSequence(new RandomFigureGenerator(FigureOptions.Ray));
          break;
        case SEGMENT_SEQUENCE:
          value = new GeneratorSequence(new RandomFigureGenerator(FigureOptions.Segment));
          break;
        case ARC_SEQUENCE:
          value = new GeneratorSequence(new RandomFigureGenerator(FigureOptions.Arc));
          break;
        default:
          throw new ArgumentException("Unsupported variable type used.");
      }
    }

    try { currentEnvironment.Define(stmt.Name, value); }
    catch (RuntimeError e) { e.AddStackTrace(stackTrace); throw e; }
    return VoidObject.Void;
  }
}