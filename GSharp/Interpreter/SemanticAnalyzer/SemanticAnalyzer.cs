namespace GSharp.Interpreter;

using GSharp.Exceptions;
using GSharp.Expression;
using GSharp.Statement;
using GSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;

public class SemanticAnalyzer : Stmt.IVisitor<GSType>, Expr.IVisitor<GSType>
{
  private readonly List<Stmt> statements;
  private SemanticErrorHandler errorHandler;
  private Func<string, List<Stmt>> importHandler;
  private VariableContext variablesContext;
  private FunctionContext builtInFunctions;
  private FunctionContext functionsContext;
  private List<string> importedFiles;
  private Stack<string> importStack;

  public SemanticAnalyzer(List<Stmt> statements, SemanticErrorHandler errorHandler, Func<string, List<Stmt>> importHandler)
  {
    void DefineBuiltIns()
    {
      const string POINT = "point";
      const string LINE = "line";
      const string RAY = "ray";
      const string SEGMENT = "segment";
      const string CIRCLE = "circle";
      const string ARC = "arc";
      const string MEASURE = "measure";
      const string INTERSECT = "intersect";
      const string COUNT = "count";
      const string RANDOMS = "randoms";
      const string POINTS = "points"; // sample figure
      const string SAMPLES = "samples";

      builtInFunctions.Define(POINT, new FunSymbol(
        POINT,
        new List<(GSType, string)>{
          (new SimpleType(TypeName.Scalar), "x"),
          (new SimpleType(TypeName.Scalar), "y")
        },
        new SimpleType(TypeName.Point)
      ));

      builtInFunctions.Define(LINE, new FunSymbol(
        LINE,
        new List<(GSType, string)>{
          (new SimpleType(TypeName.Point), "p1"),
          (new SimpleType(TypeName.Point), "p2")
        },
        new SimpleType(TypeName.Line)
      ));

      builtInFunctions.Define(RAY, new FunSymbol(
        RAY,
        new List<(GSType, string)>{
          (new SimpleType(TypeName.Point), "p1"),
          (new SimpleType(TypeName.Point), "p2")
        },
        new SimpleType(TypeName.Ray)
      ));

      builtInFunctions.Define(SEGMENT, new FunSymbol(
        SEGMENT,
        new List<(GSType, string)>{
          (new SimpleType(TypeName.Point), "p1"),
          (new SimpleType(TypeName.Point), "p2")
        },
        new SimpleType(TypeName.Segment)
      ));

      builtInFunctions.Define(CIRCLE, new FunSymbol(
        CIRCLE,
        new List<(GSType, string)>{
          (new SimpleType(TypeName.Point), "c"),
          (new SimpleType(TypeName.Measure), "r")
        },
        new SimpleType(TypeName.Circle)
      ));

      builtInFunctions.Define(ARC, new FunSymbol(
        ARC,
        new List<(GSType, string)>{
          (new SimpleType(TypeName.Point), "p1"),
          (new SimpleType(TypeName.Point), "p2"),
          (new SimpleType(TypeName.Point), "p3"),
          (new SimpleType(TypeName.Measure), "m")
        },
        new SimpleType(TypeName.Arc)
      ));

      builtInFunctions.Define(MEASURE, new FunSymbol(
        MEASURE,
        new List<(GSType, string)>{
          (new SimpleType(TypeName.Point), "p1"),
          (new SimpleType(TypeName.Point), "p2")
        },
        new SimpleType(TypeName.Measure)
      ));

      builtInFunctions.Define(INTERSECT, new FunSymbol(
        INTERSECT,
        new List<(GSType, string)>{
          (new DrawableType(), "f1"),
          (new DrawableType(), "f2")
        },
        new SequenceType(new SimpleType(TypeName.Point))
      ));


      builtInFunctions.Define(COUNT, new FunSymbol(
        COUNT,
        new List<(GSType, string)>{
          (new SequenceType(new UndefinedType()), "s")
        },
        new SimpleType(TypeName.Scalar)
      ));

      builtInFunctions.Define(RANDOMS, new FunSymbol(
        RANDOMS,
        new(),
        new SequenceType(new SimpleType(TypeName.Scalar))
      ));

      builtInFunctions.Define(POINTS, new FunSymbol(
        POINTS,
        new List<(GSType, string)>{
          (new DrawableType(), "f")
        },
        new SequenceType(new SimpleType(TypeName.Point))
      ));

      builtInFunctions.Define(SAMPLES, new FunSymbol(
        SAMPLES,
        new(),
        new SequenceType(new SimpleType(TypeName.Point))
      ));

    }

    builtInFunctions = new();
    DefineBuiltIns();
    this.statements = statements;
    this.errorHandler = errorHandler;
    this.importHandler = importHandler;
    importedFiles = new();
    importStack = new();
  }

  public void Analyze()
  {
    functionsContext = new();
    variablesContext = new();
    foreach (var stmt in statements)
      TypeCheck(stmt);
  }

  private GSType TypeCheck(Stmt stmt) => stmt.Accept(this);
  private GSType TypeCheck(Expr expr) => expr.Accept(this);

  public GSType VisitColorStmt(ColorStmt color) => new UndefinedType();
  public GSType VisitConstantStmt(ConstantStmt constantStmt)
  {
    const string CANNOT_DESTRUCTURE_ = "Cannot destructure ";

    void DefineVariableOrError(Token name, GSType type)
    {
      string VariableRedefinedError(string name)
          => $"Constant {name} is already defined in this Context";

      if (name.lexeme == "_") return;

      bool redefined = !variablesContext.Define(
          name.lexeme,
          new VariableSymbol(type, name.lexeme)
      );

      if (redefined) errorHandler(new SemanticError(name, VariableRedefinedError(name.lexeme), importStack));
    }

    void HandleSequenceType(SequenceType initType)
    {
      var elementType = initType.MostRestrictedType;
      for (int i = 0; i < constantStmt.Names.Count - 1; i++)
        DefineVariableOrError(constantStmt.Names[i], elementType);

      DefineVariableOrError(constantStmt.Names.Last(), initType);
    }

    void HandleSimpleType(SimpleType initType)
    {
      if (constantStmt.Names.Count > 1)
      {
        errorHandler(new(constantStmt.Names[0], CANNOT_DESTRUCTURE_ + initType, importStack));
        for (int i = 0; i < constantStmt.Names.Count; i++)
          DefineVariableOrError(constantStmt.Names[i], new UndefinedType());
      }
      else
        DefineVariableOrError(constantStmt.Names[0], initType);
    }

    void HandleFigureType(FigureType initType)
    {
      if (constantStmt.Names.Count > 1)
      {
        errorHandler(new(constantStmt.Names[0], CANNOT_DESTRUCTURE_ + initType, importStack));
        for (int i = 0; i < constantStmt.Names.Count; i++)
          DefineVariableOrError(constantStmt.Names[i], new UndefinedType());
      }
      else
        DefineVariableOrError(constantStmt.Names[0], initType);
    }

    void HandleDrawableType(DrawableType initType)
    {
      for (int i = 0; i < constantStmt.Names.Count; i++)
        DefineVariableOrError(constantStmt.Names[i], initType.Copy());
    }

    void HandleUndefinedType(UndefinedType initType)
    {
      for (int i = 0; i < constantStmt.Names.Count; i++)
        DefineVariableOrError(constantStmt.Names[i], initType.Copy());
    }

    var initType = TypeCheck(constantStmt.Initializer);

    switch (initType)
    {
      case DrawableType d: HandleDrawableType(d); break;
      case FigureType f: HandleFigureType(f); break;
      case SequenceType seq: HandleSequenceType(seq); break;
      case SimpleType st: HandleSimpleType(st); break;
      case UndefinedType u: HandleUndefinedType(u); break;
    }

    return new UndefinedType();
  }

  public GSType VisitDrawStmt(Draw draw)
  {
    var draweType = TypeCheck(draw.Elements);

    if (!draweType.IsDrawable())
      errorHandler(new(draw.Command, "Cannot draw " + draweType, importStack));

    return new UndefinedType();
  }

  public GSType VisitExpressionStmt(ExpressionStmt expressionStmt) => TypeCheck(expressionStmt.Expression);

  public GSType VisitFunctionStmt(Function function)
  {
    var nameTok = function.Name;
    var name = nameTok.lexeme;
    var arity = function.Parameters.Count;

    FunSymbol GetFunSymbolAndCheckBody()
    {

      void DefineParameters(List<(GSType, string)> ParamList)
      {
        void DefineParameterOrError(Token paramName, GSType type)
        {
          type ??= new UndefinedType();

          bool redefined = !variablesContext.Define(
              paramName.lexeme,
              new VariableSymbol(type, paramName.lexeme)
          );

          if (redefined)
            errorHandler(new(paramName, $"Parameter {paramName.lexeme} is already defined in this Context", importStack));
        }

        for (int i = 0; i < function.Parameters.Count; i++)
        {
          var parameter = function.Parameters[i];
          var name = parameter.Name;
          var type = parameter.typeName;


          DefineParameterOrError(name, type);

          ParamList.Add((type == null ? new UndefinedType() : type, name.lexeme));
        }
      }

      var ogFunctionContext = functionsContext;
      var ogVarContext = variablesContext;

      functionsContext = new(functionsContext);
      variablesContext = new();

      var Parameters = new List<(GSType, string)>();

      DefineParameters(Parameters);

      GSType retType = function.ReturnTypeName == null ? new UndefinedType() : function.ReturnTypeName;

      var FunSymbol = new FunSymbol(name, Parameters, retType);
      functionsContext.Define(name, FunSymbol);

      foreach(var stmt in function.Body)
      {
        if (stmt is Statement.Return ret)
        {
          var practicalRetType = TypeCheck(ret);
          if (!practicalRetType.SameTypeAs(retType)) 
          {
            errorHandler(new(function.Token, $"Function returns {practicalRetType}, but {retType} is specified as expected return type", importStack));
          }
          break;
        }
        else TypeCheck(stmt);
      }

      FunSymbol = new(name, Parameters, retType);

      functionsContext = ogFunctionContext;
      variablesContext = ogVarContext;

      return FunSymbol;
    }

    var FunSymbol = builtInFunctions.GetSymbol(name, arity) ?? functionsContext.GetSymbol(name, arity);

    if (FunSymbol != null)
    {
      errorHandler(new(nameTok, $"Function {name} is already defined in this Context", importStack));
      return new UndefinedType();
    }

    functionsContext.Define(name, GetFunSymbolAndCheckBody());

    return new UndefinedType();

  }

  public GSType VisitImportStmt(Import import)
  {
    var dir = (string)import.DirName.literal;

    if (importedFiles.Contains(dir)) return new UndefinedType();
    importedFiles.Add(dir);
    importStack.Push(dir);

    var stmts = importHandler(dir);

    if (stmts == null)
      errorHandler(new(import.Command, "File " + (string)import.DirName.literal + " not found", importStack));
    else foreach(var stmt in stmts)
    {
      TypeCheck(stmt);
    }

    importStack.Pop();

    return new UndefinedType();
  }
  public GSType VisitPrintStmt(Print print)
  {
    TypeCheck(print.Expression);

    return new UndefinedType();
  }

  public GSType VisitRestoreStmt(Restore restore) => new UndefinedType();

  public GSType VisitVarStmt(Var varStmt)
  {
    GSType Type = varStmt.TypeReference.TypeSpecifier.type switch
    {
      TokenType.POINT => TypeName.Point,
      TokenType.LINE => TypeName.Line,
      TokenType.RAY => TypeName.Ray,
      TokenType.SEGMENT => TypeName.Segment,
      TokenType.CIRCLE => TypeName.Circle,
      TokenType.ARC => TypeName.Arc,

      TokenType.POINT_SEQUENCE => new SequenceType(TypeName.Point),
      TokenType.LINE_SEQUENCE => new SequenceType(TypeName.Line),
      TokenType.RAY_SEQUENCE => new SequenceType(TypeName.Ray),
      TokenType.SEGMENT_SEQUENCE => new SequenceType(TypeName.Segment),
      TokenType.CIRCLE_SEQUENCE => new SequenceType(TypeName.Circle),
      TokenType.ARC_SEQUENCE => new SequenceType(TypeName.Arc),

      _ => throw new NotImplementedException("UNSUPPORTED TYPE IN VAR STMT")
    };

    var nameTok = varStmt.Name;
    var name = nameTok.lexeme;

    bool redefined = !variablesContext.Define(name, new VariableSymbol(Type, name));

    if (redefined)
      errorHandler(new SemanticError(nameTok, $"Variable {name} is already defined in this Context", importStack));

    return new UndefinedType();
  }

  public GSType VisitReturnStmt(GSharp.Statement.Return @return)
  {
    if (@return.Value != null) return TypeCheck(@return.Value);
    
    throw new Exception("RETURN STATEMENT WITH NULL VALUE");
  }

  public GSType VisitBinaryExpr(Binary binary)
  {
    var LeftT = TypeCheck(binary.Left);
    var RightT = TypeCheck(binary.Right);

    GSType HandleError((GSType type, string? error) tup)
    {
      if (tup.error != null)
        errorHandler(new(binary.Oper, tup.error, importStack));


      return tup.type;
    }

    switch (binary.Oper.type)
    {
      case TokenType.PLUS: return HandleError(IOperable<Add>.Operable<Add>(LeftT, RightT));
      case TokenType.MINUS: return HandleError(IOperable<Subst>.Operable<Subst>(LeftT, RightT));
      case TokenType.MUL: return HandleError(IOperable<Mult>.Operable<Mult>(LeftT, RightT));
      case TokenType.DIV: return HandleError(IOperable<Div>.Operable<Div>(LeftT, RightT));
      case TokenType.MOD: return HandleError(IOperable<Mod>.Operable<Mod>(LeftT, RightT));
      case TokenType.POWER: return HandleError(IOperable<Power>.Operable<Power>(LeftT, RightT));
      case TokenType.LESS: return HandleError(IOperable<LessTh>.Operable<LessTh>(LeftT, RightT));
      case TokenType.LESS_EQUAL: return HandleError(IOperable<LessTh>.Operable<LessTh>(LeftT, RightT));
      case TokenType.GREATER: return HandleError(IOperable<LessTh>.Operable<LessTh>(LeftT, RightT));
      case TokenType.GREATER_EQUAL: return HandleError(IOperable<LessTh>.Operable<LessTh>(LeftT, RightT));

      case TokenType.EQUAL_EQUAL:
      case TokenType.NOT_EQUAL:
        return TypeName.Scalar;

      default: throw new NotImplementedException("UNSUPPORTED OPERATOR IN ANALYZER");
    }
  }

  public GSType VisitCallExpr(Call call)
  {
    var nameTok = call.TokenAwareCalle.Token;
    var name = nameTok.lexeme;
    var argCount = call.Arguments.Count;

    var FunSymbol = builtInFunctions.GetSymbol(name, argCount) ?? functionsContext.GetSymbol(name, argCount);

    if (FunSymbol == null)
    {
      errorHandler(new(nameTok, $"Function {name} is undefined in this Context", importStack));
      return new UndefinedType();
    }

    for (int i = 0; i < argCount; i++)
    {
      var paramType = FunSymbol.Parameters[i].Type;
      var argType = TypeCheck(call.Arguments[i]);

      if (!paramType.SameTypeAs(argType))
        errorHandler(new(nameTok, $"Function {name} takes {paramType} as its #{i+1} parameter, {argType} passed instead", importStack));
    }

    return FunSymbol.ReturnType;
  }

  public GSType VisitConditionalExpr(Conditional conditional)
  {
    TypeCheck(conditional.Condition);

    var thenBranch = TypeCheck(conditional.ThenBranch);
    var elseBranch = TypeCheck(conditional.ElseBranch);

    if (!thenBranch.SameTypeAs(elseBranch))
    {
      errorHandler(new(conditional.If, $"Conditional expression must have equal return types, {thenBranch} and {elseBranch} returned instead", importStack));
      return new UndefinedType();
    }
    return thenBranch.GetMostRestrictedOrError(elseBranch);
  }

  public GSType VisitEmptyExpr(Empty empty)
  {
    return new UndefinedType();
  }

  public GSType VisitGroupingExpr(Grouping grouping)
  {
    return TypeCheck(grouping.Expression);
  }

  public GSType VisitIntRangeExpr(IntRange intRange)
  {
    const string ERROR = " limit in range expression must be integer literal";
    var L = double.Parse((string)intRange.Left.literal);
    if (!double.IsInteger(L))
      errorHandler(new(intRange.Left, "Left" + ERROR, importStack));
    
    if (intRange.Right != null)
    {
      var R = double.Parse((string)intRange.Right.literal);
      
      if (!double.IsInteger(R))
        errorHandler(new(intRange.Right, "Right" + ERROR, importStack));

      if (intRange.LeftNegative) L = -L;
      if (intRange.RightNegative) R = -R;
      if (L > R)
        errorHandler(new(
          intRange.Dots, 
          $"Invalid range value in left {L} expected to be less or equal to value in right {R}", 
          importStack
        ));
    }

    return new SequenceType(TypeName.Scalar);
  }

  public GSType VisitLetInExpr(LetIn letIn)
  {
    var ogVarContext = new VariableContext(variablesContext);
    var ogFunctionContext = new FunctionContext(functionsContext);

    GSType retType = null;

    for (int i = 0; i < letIn.Stmts.Count; i++)
    {
      if (letIn.Stmts[i] is Statement.Return retStmt)
      {
        retType = TypeCheck(retStmt.Value); 
      }
      else TypeCheck(letIn.Stmts[i]);
    }

    if (retType == null) throw new Exception("RETURN STATEMENT NEVER REACHED");

    variablesContext = ogVarContext;
    functionsContext = ogFunctionContext;

    return retType;
  }

  public GSType VisitLiteralExpr(Literal literal)
  {
    if (literal.Value is INumericLiteral) return new SimpleType(TypeName.Scalar);
    else if (literal.Value is string) return new SimpleType(TypeName.String);
    else if (literal.Value is bool) return new SimpleType(TypeName.Scalar);
    
    throw new NotImplementedException("UNSUPPORTED LITERAL TYPE");
  }

  public GSType VisitLogicalExpr(Logical logical)
  {
    TypeCheck(logical.Left);
    TypeCheck(logical.Right);

    return new SimpleType(TypeName.Scalar);
  }

  public GSType VisitSequenceExpr(Sequence sequence)
  {
    SequenceType seqT = new();

    foreach(var item in sequence.Items)
    {
      var type = TypeCheck(item);
      
      if(item is IntRange)
        type = new SimpleType(TypeName.Scalar);
      
      var (accepted, errorMessage) = seqT.AcceptNewType(type);

      if (!accepted)
        errorHandler(new(sequence.Brace, errorMessage, importStack));
    }

    return seqT;
  }

  public GSType VisitUnaryExpr(Unary unary)
  {
    var left = TypeCheck(unary.Right);

    if (unary.Token.type == TokenType.MINUS)
    {
      var (type, error) = IOperable<Mult>.Operable<Mult>(new SimpleType(TypeName.Scalar), left);
      if (error != null)
        errorHandler(new(unary.Token, $"Cannot apply minus unary operand to " + left.ToString(), importStack));
      return type;
    }
    
    else if (unary.Token.type == TokenType.NOT)
      return new SimpleType(TypeName.Scalar);

    throw new NotImplementedException("UNSUPPORTED UNARY OPERATOR");
  }

  public GSType VisitUndefinedExpr(Undefined undefined)
  {
    return new UndefinedType();
  }

  public GSType VisitVariableExpr(Variable variable)
  {
    var nameTok = variable.Name;
    var name = nameTok.lexeme;

    var symbol = variablesContext.GetSymbol(name);

    if (symbol == null)
    {
      errorHandler(new(nameTok, $"Variable {name} is undefined in this Context", importStack));
      return new UndefinedType();
    }

    return symbol.Type;  
  }

}