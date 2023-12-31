using GSharp.Exceptions;
using GSharp.Expression;
using GSharp.Statement;
using static GSharp.TokenType;
using static GSharp.Exceptions.ParseErrorType;
using System;
using System.Collections.Generic;
using System.Linq;

using GSharp.Types;

namespace GSharp.Parser;

// <summary>
// Parses GSharp code to either a list of statements.
// </summary>
public class Parser
{
  private class InternalParseError : Exception
  {
    public ParseErrorType? parseErrorType { get; }

    public InternalParseError(ParseErrorType? parseErrorType)
    {
      this.parseErrorType = parseErrorType;
    }
  }

  private readonly ParseErrorHandler parseErrorHandler;
  private readonly List<Token> tokens;

  /// <summary>
  /// Scans and parses the given program, to prepare for execution or inspection.
  ///
  /// This method is useful for inspecting the AST or perform other validation of the
  /// internal state after parsing a given program. It is also used for interpreting
  /// the statements.
  /// </summary>
  public static ScanAndParseResult ScanAndParse(string source, ScanErrorHandler scanErrorHandler, ParseErrorHandler parseErrorHandler, Stack<string> importTrace)
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
      return ScanAndParseResult.scanErrorOccurred;
    }

    // ...
    // parsing phase
    // ...

    bool hasParseErrors = false;
    var parser = new Parser(tokens, parseError =>
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
      return ScanAndParseResult.parseErrorEncountered;
    }

    if (syntax is List<Stmt> stmts)
    {
      return ScanAndParseResult.OfStmts(stmts);
    }
    else
    {
      throw new IllegalStateException($"syntax expected to be List<Stmt>, not {syntax}", null);
    }
  }

  private int current;
  private Stack<string> importTrace;

  public Parser(List<Token> tokens, ParseErrorHandler parseErrorHandler, Stack<string> importTrace)
  {
    this.tokens = tokens;
    this.parseErrorHandler = parseErrorHandler;
    this.importTrace = importTrace;
  }

  public IList<Stmt> ParseStmts()
  {
    var statements = new List<Stmt>();

    while (!IsAtEnd())
    {
      statements.Add(Declaration());
    }

    return statements;
  }

  private Expr Expression()
  {
    if (Match(IF)) return IfExpression(Previous());
    if (Match(LET)) return LetInExpression(Previous());
    return Assignment();
  }

  private Stmt Declaration()
  {
    try
    {
      if (IsFunction())
      {
        return FunDeclaration("function");
      }

      if (Match(POINT, LINE, SEGMENT, RAY, CIRCLE, ARC))
      {
        return VarDeclaration(Previous());
      }

      if (Match(POINT_SEQUENCE, LINE_SEQUENCE, SEGMENT_SEQUENCE, RAY_SEQUENCE, CIRCLE_SEQUENCE, ARC_SEQUENCE))
      {
        return VarDeclaration(Previous());
      }

      if (IsConstant() && Match(IDENTIFIER))
      {
        return ConstDeclaration();
      }

      return Statement();
    }
    catch (InternalParseError)
    {
      Synchronize();
      return null;
    }
  }

  private Stmt Statement()
  {
    if (Match(FOR)) return ForStatement();
    if (Match(WHILE)) return WhileStatement();
    if (Match(DRAW)) return DrawStatement(Previous());
    if (Match(COLOR)) return ColorStatement(Previous());
    if (Match(RESTORE)) return RestoreStatement(Previous());
    if (Match(IMPORT)) return ImportStatement(Previous());
    if (Match(PRINT)) return PrintStatement(Previous());

    return ExpressionStatement();
  }

  private Stmt ExpressionStatement()
  {
    Expr expr = Expression();
    Consume(SEMICOLON, "Expected ';' after expression.", MISSING_SEMICOLON);
    return new ExpressionStmt(expr);
  }

  private Stmt ForStatement()
  {
    Consume(LEFT_PARENTESIS, "Expected '(' after 'for'.");

    Stmt initializer;
    if (Match(SEMICOLON))
    {
      initializer = null;
    }
    else if (Match(POINT, LINE, SEGMENT, RAY, CIRCLE, ARC))
    {
      initializer = VarDeclaration(Previous());
    }
    else if (Match(POINT_SEQUENCE, LINE_SEQUENCE, SEGMENT_SEQUENCE, RAY_SEQUENCE, CIRCLE_SEQUENCE, ARC_SEQUENCE))
    {
      initializer = VarDeclaration(Previous());
    }
    else if (IsConstant() && Match(IDENTIFIER))
    {
      initializer = ConstDeclaration();
    }
    else
    {
      initializer = ExpressionStatement();
    }

    Expr condition = null;
    if (!Check(SEMICOLON))
    {
      condition = Expression();
    }

    Consume(SEMICOLON, "Expected ';' after loop condition.");

    Expr increment = null;
    if (!Check(RIGHT_PARENTESIS))
    {
      increment = Expression();
    }

    Consume(RIGHT_PARENTESIS, "Expected ')' after for clauses.");

    Stmt body = ExpressionStatement();

    if (increment != null)
    {
      body = new Block(new List<Stmt>{
        body,
        new ExpressionStmt(increment)
      });
    }

    condition ??= new Literal(true);

    body = new While(condition, body);

    if (initializer != null)
    {
      body = new Block(new List<Stmt> { initializer, body });
    }

    return body;
  }

  private Stmt WhileStatement()
  {
    Consume(LEFT_PARENTESIS, "Expected '(' after 'while'.");
    Expr condition = Expression();
    Consume(RIGHT_PARENTESIS, "Expected ')' after condition.");
    Stmt body = ExpressionStatement();

    return new While(condition, body);
  }

  private Expr IfExpression(Token ifTk)
  {
    Expr condition = Expression();
    Consume(THEN, "Expected if <expr> then <expr> else <expr>.");

    Expr thenBranch = Expression();
    Consume(ELSE, "Expected if <expr> then <expr> else <expr>.");
    Expr elseBranch = Expression();
    return new Conditional(ifTk, condition, thenBranch, elseBranch);
  }

  private Stmt FunDeclaration(string kind)
  {
    TypeName? getTypeNameOrError(Token typeSpecifier)
    {
      return typeSpecifier.lexeme switch
      {
        "point" => TypeName.Point,
        "line" => TypeName.Line,
        "ray" => TypeName.Ray,
        "segment" => TypeName.Segment,
        "circle" => TypeName.Circle,
        "arc" => TypeName.Arc,

        "scalar" => TypeName.Scalar,
        "measure" => TypeName.Measure,
        "string" => TypeName.String,

        _ => throw Error(typeSpecifier, "Expecting type name")
      };
    }

    Token name = Consume(IDENTIFIER, "Expected " + kind + " name.");
    Consume(LEFT_PARENTESIS, "Expected '(' after " + kind + " name.");
    var parameters = new List<Parameter>();

    BlockReservedIdentifiers(name);

    if (!Check(RIGHT_PARENTESIS))
    {
      do
      {
        if (parameters.Count >= 1024)
        {
          Error(Peek(), "Can't have more than 1024 parameters.");
          return null;
        }

        Token parameterName = Consume(IDENTIFIER, "Expected parameter name.");

        BlockReservedIdentifiers(parameterName);

        Token parameterTypeSpecifier = null;

        TypeName? typeName = null;

        // parameters can optionally use a specific type
        // if the type is not provided, the compiler will try to infer the type based on the usage
        if (Match(TWO_DOTS))
        {
          if (Match(IDENTIFIER) || Match(POINT) || Match(LINE) || Match(RAY) || Match(SEGMENT) || Match(CIRCLE) || Match(ARC))
            parameterTypeSpecifier = Previous();

          else throw Error(Peek(), "Expecting type name", null);

          typeName = getTypeNameOrError(parameterTypeSpecifier);
        }

        parameters.Add(new Parameter(parameterName, new TypeReference(parameterTypeSpecifier), typeName));
      } while (Match(COMMA));
    }

    Consume(RIGHT_PARENTESIS, "Expected ')' after parameters.");

    Token returnTypeSpecifier = null;
    TypeName? returnTypeName = null;

    if (Match(TWO_DOTS))
    {
      if (Match(IDENTIFIER) || Match(POINT) || Match(LINE) || Match(RAY) || Match(SEGMENT) || Match(CIRCLE) || Match(ARC))
        returnTypeSpecifier = Previous();

      else throw Error(Peek(), "Expecting type name", null);
      returnTypeName = getTypeNameOrError(returnTypeSpecifier);
    }

    Consume(EQUAL, "Expected '=' before function's body.");

    List<Stmt> body = new List<Stmt>();

    Stmt returnValue = ReturnStatement();

    body.Add(returnValue);

    Consume(SEMICOLON, "Expected ';' after function declaration.");
    return new Function(name, parameters, body, new TypeReference(returnTypeSpecifier), returnTypeName);
  }

  private Stmt DrawStatement(Token command)
  {
    Expr elements = Expression();

    Token nameId = null;
    if (Check(STRING))
    {
      nameId = Consume(STRING, "Expected string.");
    }

    Consume(SEMICOLON, "Expected 'label' after draw command.");
    return new Draw(command, elements, nameId);
  }

  private Stmt ColorStatement(Token command)
  {
    if (Match(BLUE, RED, YELLOW, GREEN, CYAN, MAGENTA, WHITE, GRAY, BLACK))
    {
      Token color = Previous();
      Consume(SEMICOLON, "Expected ';' after color command.");
      return new ColorStmt(command, color);
    }
    else
    {
      Error(Peek(), "Expected a valid color.");
      return null;
    }
  }

  private Stmt RestoreStatement(Token command)
  {
    Consume(SEMICOLON, "Expected ';' after restore command.");
    return new Restore(command);
  }

  private Stmt ImportStatement(Token command)
  {
    Token dirName = Consume(STRING, "Expected directory name.");
    Consume(SEMICOLON, "Expected ';' after import ");
    return new Import(command, dirName);
  }

  private Stmt PrintStatement(Token command)
  {
    Expr value = Expression();

    Token label = null;
    if (Match(STRING))
    {
      label = Previous();
    }

    Consume(SEMICOLON, "Expected ';' after print expressions.");
    return new Print(command, value, label);
  }

  private Stmt VarDeclaration(Token type)
  {
    Token name = Consume(IDENTIFIER, "Expected variable name.");

    BlockReservedIdentifiers(name);

    Expr initializer = null;

    Consume(SEMICOLON, "Expected ';' after variable declaration.");
    return new Var(name, initializer, new TypeReference(type));
  }

  private Stmt ReturnStatement()
  {
    Expr value = Expression();
    return new Statement.Return(null, value);
  }

  private List<Token> GetNames()
  {
    List<Token> names = new List<Token> { Previous() };
    if (!Check(EQUAL))
    {
      Consume(COMMA, "Expected ',' before variable name.");
      do
      {
        if (names.Count >= 1024)
        {
          Error(Peek(), "Can't have more than 1024 variables.");
          return null;
        }
        names.Add(Consume(IDENTIFIER, "Expected variable name."));
      } while (Match(COMMA));
    }
    return names;
  }

  private Stmt ConstDeclaration()
  {
    List<Token> constNames = GetNames();
    Consume(EQUAL, "Expected '=' after constant name.");
    Expr initializer = Expression();
    Consume(SEMICOLON, "Expected ';' after constant declaration.");

    return new ConstantStmt(constNames, initializer);
  }

  private Expr SequenceDeclaration(Token openBraceTk)
  {
    List<Expr> items = new List<Expr>();
    if (!Check(RIGHT_BRACE))
    {
      do
      {
        if (items.Count >= 1024)
        {
          Error(Peek(), "Can't have more than 1024 items.");
        }

        if (current + 2 < tokens.Count && Check(MINUS) && CheckNext(NUMBER) && tokens[current + 2].type == DOTS)
        {
          Consume(MINUS, "Expected '-' by condition");
          Token left = Consume(NUMBER, "Range limit must be a integer constant.");
          Token dots = Consume(DOTS, "Expected '...' after left limit of range.");

          Token right = null; bool rightNeg = false;
          if (Check(MINUS) && CheckNext(NUMBER))
          {
            rightNeg = true;
            Consume(MINUS, "Expected '-' by condition");
            right = Consume(NUMBER, "Range limit must be a integer constant.");
          }
          else if (Check(NUMBER))
          {
            right = Consume(NUMBER, "Range limit must be a integer constant.");
          }

          items.Add(new IntRange(left, dots, right, true, rightNeg));
        }
        else if (Check(NUMBER) && CheckNext(DOTS))
        {
          Token left = Consume(NUMBER, "Range limit must be a integer constant.");
          Token dots = Consume(DOTS, "Expected '...' after left limit of range.");

          Token right = null; bool rightNeg = false;
          if (Check(MINUS) && CheckNext(NUMBER))
          {
            rightNeg = true;
            Consume(MINUS, "Expected '-' by condition");
            right = Consume(NUMBER, "Range limit must be a integer constant.");
          }
          else if (Check(NUMBER))
          {
            right = Consume(NUMBER, "Range limit must be a integer constant.");
          }

          items.Add(new IntRange(left, dots, right, false, rightNeg));
        }
        else
        {
          items.Add(Expression());
        }
      } while (Match(COMMA));
    }

    Consume(RIGHT_BRACE, "Expected '}' after sequence declaration.");

    return new Sequence(openBraceTk, items);
  }

  private Expr LetInExpression(Token Let)
  {

    List<Stmt> body = new List<Stmt>();

    var instructions = LetInBlock();
    foreach (var instruction in instructions)
    {
      body.Add(instruction);
    }

    Stmt returnValue = ReturnStatement();

    body.Add(returnValue);

    return new LetIn(Let, body);
  }

  private List<Stmt> LetInBlock()
  {
    var statements = new List<Stmt>();
    while (!Check(IN) && !IsAtEnd())
    {
      statements.Add(Declaration());
    }
    Consume(IN, "Expected 'IN' after block.");
    return statements;
  }

  private Expr Assignment()
  {
    Expr expr = UnaryPostfix();

    if (Match(ASSIGN))
    {
      Token assignEquals = Previous();
      Expr value = Assignment();

      if (expr is Variable variable)
      {
        return new Assign(variable, value);
      }

      Error(assignEquals, "Invalid assignment target.");
    }
    else if (Match(PLUS_EQUAL, MINUS_EQUAL))
    {
      Token token = Previous();
      Expr value = Term();

      if (expr is Variable variable)
      {
        Expr newValue = new Binary(variable, token, value);

        return new Assign(variable, newValue);
      }

      Error(token, "Invalid assignment target.");
    }

    return expr;
  }

  private Expr UnaryPostfix()
  {
    Expr expr = Or();

    if (Match(PLUS_PLUS))
    {
      Token increment = Previous();

      if (expr is Variable variable)
      {
        return new UnaryPostfix(variable, variable.Name, increment);
      }

      Error(increment, $"Can only increment variables, not {expr}");
    }
    else if (Match(MINUS_MINUS))
    {
      Token decrement = Previous();

      if (expr is Variable variable)
      {
        return new UnaryPostfix(variable, variable.Name, decrement);
      }

      Error(decrement, $"Can only decrement variables, not {expr}");
    }

    return expr;
  }

  private Expr Or()
  {
    Expr expr = And();
    while (Match(OR))
    {
      Token oper = Previous();
      Expr right = And();
      expr = new Logical(expr, oper, right);
    }
    return expr;
  }

  private Expr And()
  {
    Expr expr = Equality();
    while (Match(AND))
    {
      Token oper = Previous();
      Expr right = Equality();
      expr = new Logical(expr, oper, right);
    }
    return expr;
  }

  private Expr Equality()
  {
    Expr expr = Comparison();
    while (Match(NOT_EQUAL, EQUAL_EQUAL))
    {
      Token oper = Previous();
      Expr right = Comparison();
      expr = new Binary(expr, oper, right);
    }
    return expr;
  }

  private Expr Comparison()
  {
    Expr expr = Term();
    while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
    {
      Token oper = Previous();
      Expr right = Term();
      expr = new Binary(expr, oper, right);
    }
    return expr;
  }

  private Expr Term()
  {
    Expr expr = Factor();
    while (Match(MINUS, PLUS))
    {
      Token oper = Previous();
      Expr right = Factor();
      expr = new Binary(expr, oper, right);
    }
    return expr;
  }

  private Expr Factor()
  {
    Expr expr = UnaryPrefix();
    while (Match(DIV, MUL, MOD, POWER))
    {
      Token oper = Previous();
      Expr right = UnaryPrefix();
      expr = new Binary(expr, oper, right);
    }
    return expr;
  }


  private Expr UnaryPrefix()
  {
    if (Match(NOT, MINUS))
    {
      Token oper = Previous();
      Expr right = UnaryPrefix();

      // we detect MINUS + NUMBER here and convert it to a single Literal()
      // instead of retaining it as a unary prefix expression
      if (oper.type == MINUS && right is Literal rightLiteral)
      {
        if (rightLiteral.Value is INumericLiteral numericLiteral)
        {
          return new Literal(NumberParser.MakeNegative(numericLiteral));
        }
        else if (rightLiteral.Value is null)
        {
          Error(Peek(), "Unary minus operator does not support null operand.");
          return new Literal(null);
        }
        else
        {
          throw new ArgumentException($"Type {rightLiteral.Value.GetType().Name} not supported.");
        }
      }
      else
      {
        return new UnaryPrefix(oper, right);
      }
    }

    return IndexingOrCall();
  }

  private Expr IndexingOrCall()
  {
    Expr expr = Primary();

    while (true)
    {
      if (Match(LEFT_PARENTESIS))
      {
        expr = FinishCall(expr);
      }
      else if (Match(LEFT_SQUARE_BRACKET))
      {
        expr = FinishIndex(expr);
      }
      else
      {
        break;
      }
    }

    return expr;
  }

  private Expr FinishCall(Expr calle)
  {
    List<Expr> arguments = new List<Expr>();
    if (!Check(RIGHT_PARENTESIS))
    {
      do
      {
        if (arguments.Count >= 1024)
        {
          Error(Peek(), "Can't have more than 1024 arguments.");
        }
        arguments.Add(Expression());
      } while (Match(COMMA));
    }

    Token paren = Consume(RIGHT_PARENTESIS, "Expected ')' after arguments.");

    return new Call(calle, paren, arguments);
  }

  private Expr FinishIndex(Expr indexee)
  {
    Expr argument = Expression();
    Token closingBracket = Consume(RIGHT_SQUARE_BRACKET, "Expected ']' after index argument.");

    return new Index(indexee, closingBracket, argument);
  }

  private Expr Primary()
  {
    if (Match(FALSE)) return new Literal(false);
    if (Match(TRUE)) return new Literal(true);
    if (Match(UNDEFINED)) return new Undefined();

    if (Match(NUMBER))
    {
      // numbers are retained as string in the scanning phase
      // to properly be abel to parse negative numbers in the parsing stage
      INumericLiteral numericLiteral = NumberParser.Parse((NumericToken)Previous());
      return new Literal(numericLiteral);
    }

    if (Match(STRING))
    {
      return new Literal(Previous().literal);
    }

    if (Match(IDENTIFIER))
    {
      return new Variable(Previous());
    }

    if (Match(POINT, LINE, SEGMENT, RAY, CIRCLE, ARC))
    {
      return new Variable(Previous());
    }

    if (Match(LEFT_PARENTESIS))
    {
      Expr expr = Expression();
      Consume(RIGHT_PARENTESIS, "Expected ')' after expression.");
      return new Grouping(expr);
    }

    if (Match(LEFT_BRACE))
    {
      return SequenceDeclaration(Previous());
    }

    if (Check(SEMICOLON))
    {
      // bare semicolon, no expression inside
      // to avoid having to handle pesky null exceptions all over
      // the code base, we have a dedicated expression for this
      return new Empty();
    }

    throw Error(Peek(), "Expected expression.");
  }

  private bool Match(params TokenType[] types)
  {
    // matches the given token type(s), at the current position
    // if a matching token is found, it gets consumed
    // returns 'true' if a matching token was found and consumed, 'false' otherwise
    // for a non-consuming of this see 'Check'

    foreach (TokenType type in types)
    {
      if (Check(type))
      {
        Advance();
        return true;
      }
    }

    return false;
  }

  private Token Consume(TokenType type, string message, ParseErrorType? parseErrorType = null)
  {
    // matches the given token type at the current position, expecting a match
    // if the current token does not match, an exception is thrown
    // returns the matched token, or throw a exception otherwise

    if (Check(type))
    {
      return Advance();
    }

    throw Error(Peek(), message, parseErrorType);
  }

  private bool Check(TokenType type)
  {
    // non-consuming version of Match
    // check if the current token matches the provided TokenType
    // but does not consume the token even if it matches
    // returns 'true' if it matches, 'false' otherwise

    if (IsAtEnd())
    {
      return false;
    }

    return Peek().type == type;
  }

  private bool CheckNext(TokenType type)
  {
    if (IsAtEnd()) return false;
    return PeekNext().type == type;
  }

  private bool IsFunction()
  {
    // check if the current stage corresponds to a function declaration

    if (Check(IDENTIFIER) && CheckNext(LEFT_PARENTESIS))
    {
      for (int i = current; tokens[i].type != EOF; i++)
      {
        if (tokens[i].type == RIGHT_PARENTESIS)
        {
          return tokens[i + 1].type == EQUAL || tokens[i + 1].type == TokenType.TWO_DOTS;
        }
      }
    }

    return false;
  }

  private bool IsConstant()
  {
    // check if the current stage correspond to a constant declaration

    if (Check(IDENTIFIER) && CheckNext(LEFT_PARENTESIS))
    {
      return false;
    }

    for (int i = current; tokens[i].type != EOF; i++)
    {
      if (tokens[i].type == IDENTIFIER || tokens[i].type == COMMA)
        continue;

      if (tokens[i].type == EQUAL)
        break;

      return false;
    }

    return Check(IDENTIFIER);
  }

  private Token Advance()
  {
    // advance the current position with one step
    // if the position is already at the end of the stream
    // this method does nothing
    // also returns the token at the stream position before advancing it

    if (!IsAtEnd())
    {
      current++;
    }

    return Previous();
  }

  private bool IsAtEnd()
  {
    return Peek().type == EOF;
  }

  private Token Peek()
  {
    // returns the token at the current position

    return tokens[current];
  }

  private Token PeekNext()
  {
    // return the token right after the current position

    return tokens[current + 1];
  }

  private Token Previous()
  {
    // returns the token right before the current position

    return tokens[current - 1];
  }

  private InternalParseError Error(Token token, string message, ParseErrorType? parseErrorType = null)
  {
    parseErrorHandler(new ParseError(message, token, importTrace, parseErrorType));

    return new InternalParseError(parseErrorType);
  }

  private void Synchronize()
  {
    // synchronizes the parser after an InternalParseError has occurred.
    // do a best-effort attempt to recover from the current state
    // we try to forward to the end of the current statement

    Advance();

    while (!IsAtEnd())
    {
      if (Previous().type == SEMICOLON)
        return;

      switch (Peek().type)
      {
        case IF:
        case DRAW:
        case POINT:
        case CIRCLE:
        case ARC:
        case RAY:
        case LINE:
        case SEGMENT:
        case IMPORT:
        case LET:
          return;
      }

      Advance();
    }
  }

  private void BlockReservedIdentifiers(Token token)
  {
    // throws and Error if the given token represents a reserved keyword.
    if (Scanner.ReservedKeywordStrings.Contains(token.lexeme))
    {
      throw Error(token, "Reserved keyword encountered", RESERVED_WORD_ENCOUNTERED);
    }
  }
}