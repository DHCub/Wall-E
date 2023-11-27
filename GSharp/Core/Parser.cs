namespace GSharp.Core;


using System;
using System.Collections.Generic;
using static TokenType;

public class Parser
{
  private class ParseError : Exception { };

  private int current = 0;

  private readonly ILogger logger;
  public readonly List<Token> tokens;

  public Parser(ILogger logger, List<Token> tokens)
  {
    this.logger = logger;
    this.tokens = tokens;
  }

  public List<Statement.Stmt> Parse()
  {
    List<Statement.Stmt> result = new List<Statement.Stmt>();
    while (!IsAtEnd())
    {
      result.Add(Declaration());
    }
    return result;
  }

  private Expression.Expr Expression()
  {
    if (Match(IF)) return IfExpression(Previous());
    if (Match(LET)) return LetInExpression(Previous());
    return Assignment();
  }

  private Statement.Stmt Declaration()
  {
    try
    {
      if (IsFunction())
      {
        return FunDeclaration();
      }

      if (Match(POINT, LINE, SEGMENT, RAY, CIRCLE, ARC))
      {
        return VarDeclaration(Previous());
      }

      if (IsConstant() && Match(IDENTIFIER))
      {
        return ConstDeclaration();
      }
      return Statement();
    }
    catch
    {
      Synchronize();
      return null;
    }
  }

  private Statement.Stmt Statement()
  {
    if (Match(DRAW))
    {
      return DrawStatement(Previous());
    }

    if (Match(COLOR))
    {
      return ColorStatement();
    }

    if (Match(RESTORE))
    {
      return RestoreStatement();
    }

    if (Match(IMPORT))
    {
      return ImportStatement();
    }

    if (Match(PRINT))
    {
      return PrintStatement();
    }

    return ExpressionStatement();
  }

  private Statement.Stmt ExpressionStatement()
  {
    Expression.Expr expr = Expression();
    Eat(SEMICOLON, "Expected ';' after expression.");
    return new Statement.Expression(expr);
  }

  private Expression.Expr IfExpression(Token ifTk)
  {
    Expression.Expr condition = Expression();
    Eat(THEN, "Expected if <expr> then <expr> else <expr>.");

    Expression.Expr thenBranch = Expression();
    Eat(ELSE, "Expected if <expr> then <expr> else <expr>.");
    Expression.Expr elseBranch = Expression();
    return new Expression.Conditional(ifTk, condition, thenBranch, elseBranch);
  }

  private Statement.Stmt FunDeclaration()
  {
    Token name = Eat(IDENTIFIER, "Expected function name.");
    Eat(LEFT_PARENTESIS, "Expected '(' after function name.");
    List<Token> parameters = new List<Token>();
    if (!Check(RIGHT_PARENTESIS))
    {
      do
      {
        if (parameters.Count >= 1024)
        {
          Error(Peek(), "Can't have more than 1024 parameters.");
          return null;
        }
        parameters.Add(Eat(IDENTIFIER, "Expected parameter name."));
      } while (Match(COMMA));
    }
    Eat(RIGHT_PARENTESIS, "Expected ')' after parameters.");
    Eat(EQUAL, "Expected '=' before function's body.");
    Expression.Expr body = Expression();
    Eat(SEMICOLON, "Expected ';' after function declaration.");
    return new Statement.Function(name, parameters, body);
  }

  private Statement.Stmt DrawStatement(Token DrawCommand)
  {
    Expression.Expr elements = Expression();

    Token nameId = null;
    if (Check(STRING))
    {
      nameId = Eat(STRING, "Expected string.");
    }

    Eat(SEMICOLON, "Expected 'l' after draw command.");
    return new Statement.Draw(elements, DrawCommand, nameId);
  }

  private Statement.Stmt ColorStatement()
  {
    if (Match(BLUE, RED, YELLOW, GREEN, CYAN, MAGENTA, WHITE, GRAY, BLACK))
    {
      Token color = Previous();
      Eat(SEMICOLON, "Expected ';' after color command.");
      return new Statement.Color(color);
    }
    else
    {
      Error(Peek(), "Expected a valid color.");
      return null;
    }
  }

  private Statement.Stmt RestoreStatement()
  {
    Eat(SEMICOLON, "Expected ';' after restore command.");
    return null;
  }

  private Statement.Stmt ImportStatement()
  {
    Token dirName = Eat(STRING, "Expected directory name.");
    Eat(SEMICOLON, "Expected ';' after import statement.");
    return new Statement.Import(dirName);
  }

  private Statement.Stmt PrintStatement()
  {
    List<Expression.Expr> printe = new List<Expression.Expr>();
    while (!Check(SEMICOLON) && !IsAtEnd())
    {
      printe.Add(Expression());
    }
    Eat(SEMICOLON, "Expected ';' after print statement.");
    return new Statement.Print(printe);
  }

  private Statement.Stmt VarDeclaration(Token type)
  {
    bool isSequence = false;
    if (Match(SEQUENCE))
    {
      isSequence = true;
    }

    Token name = Eat(IDENTIFIER, "Expected variable name.");

    Statement.Stmt initializer = null;

    Eat(SEMICOLON, "Expected ';' after variable declaration.");
    return new Statement.Var(type, name, initializer, isSequence);
  }

  private List<Token> GetNames()
  {
    List<Token> names = new List<Token> { Previous() };
    if (!Check(EQUAL))
    {
      Eat(COMMA, "Expected ',' before variable name.");
      do
      {
        if (names.Count >= 1024)
        {
          Error(Peek(), "Can't have more than 1024 variables.");
          return null;
        }
        names.Add(Eat(IDENTIFIER, "Expected variable name."));
      } while (Match(COMMA));
    }
    return names;
  }

  private Statement.Stmt ConstDeclaration()
  {
    List<Token> constNames = GetNames();
    Eat(EQUAL, "Expected '=' after constant name.");
    Expression.Expr initializer = Expression();
    Eat(SEMICOLON, "Expected ';' after constant declaration.");
    return new Statement.Constant(constNames, initializer);
  }

  private List<Statement.Stmt> Instructions()
  {
    List<Statement.Stmt> instructions = new List<Statement.Stmt>();
    while (!Check(IN) && !IsAtEnd())
    {
      instructions.Add(Declaration());
    }
    return instructions;
  }

  private Expression.Expr SequenceDeclaration(Token openBraceTk)
  {
    List<Expression.Expr> items = new List<Expression.Expr>();
    if (!Check(RIGHT_BRACE))
    {
      do
      {
        if (items.Count >= 1024)
        {
          Error(Peek(), "Can't have more than 1024 items.");
        }

        if (CheckNext(DOTS))
        {
          Token left = Eat(NUMBER, "Range limit must be a integer constant.");
          Eat(DOTS, "Expected '...' after left limit of range.");

          Token right = null;
          if (Check(NUMBER))
            right = Eat(NUMBER, "Range limit must be a integer constant.");

          items.Add(new GSharp.Expression.Range(left, right));
        }
        else
        {
          items.Add(Expression());
        }
      } while (Match(COMMA));
    }

    Eat(RIGHT_BRACE, "Expected '}' after sequence declaration.");

    return new Expression.Sequence(openBraceTk, items);
  }

  private Expression.Expr LetInExpression(Token letTk)
  {
    var instructions = Instructions();
    Eat(IN, "Expected 'in' at end of 'let-in' expression.");
    Expression.Expr body = Expression();
    return new Expression.LetIn(letTk, instructions, body);
  }

  private Expression.Expr Assignment()
  {
    Expression.Expr expr = Or();
    return expr;
  }

  private Expression.Expr Or()
  {
    Expression.Expr expr = And();
    while (Match(OR))
    {
      Token oper = Previous();
      Expression.Expr right = And();
      expr = new Expression.Logical(expr, oper, right);
    }
    return expr;
  }

  private Expression.Expr And()
  {
    Expression.Expr expr = Equality();
    while (Match(AND))
    {
      Token oper = Previous();
      Expression.Expr right = Equality();
      expr = new Expression.Logical(expr, oper, right);
    }
    return expr;
  }

  private Expression.Expr Equality()
  {
    Expression.Expr expr = Comparison();
    while (Match(NOT_EQUAL, EQUAL_EQUAL))
    {
      Token oper = Previous();
      Expression.Expr right = Comparison();
      expr = new Expression.Binary(expr, oper, right);
    }
    return expr;
  }

  private Expression.Expr Comparison()
  {
    Expression.Expr expr = Term();
    while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
    {
      Token oper = Previous();
      Expression.Expr right = Term();
      expr = new Expression.Binary(expr, oper, right);
    }
    return expr;
  }

  private Expression.Expr Term()
  {
    Expression.Expr expr = Factor();
    while (Match(MINUS, PLUS))
    {
      Token oper = Previous();
      Expression.Expr right = Factor();
      expr = new Expression.Binary(expr, oper, right);
    }
    return expr;
  }

  private Expression.Expr Factor()
  {
    Expression.Expr expr = Unary();
    while (Match(DIV, MUL))
    {
      Token oper = Previous();
      Expression.Expr right = Unary();
      expr = new Expression.Binary(expr, oper, right);
    }
    return expr;
  }

  private Expression.Expr Unary()
  {
    if (Match(NOT, MINUS))
    {
      Token oper = Previous();
      Expression.Expr right = Unary();
      return new Expression.Unary(oper, right);
    }

    return Call();
  }

  private Expression.Expr FinishCall(Expression.Expr calle)
  {
    List<Expression.Expr> parameters = new List<Expression.Expr>();
    if (!Check(RIGHT_PARENTESIS))
    {
      do
      {
        if (parameters.Count >= 1024)
        {
          Error(Peek(), "Can't have more than 1024 parameters.");
        }
        parameters.Add(Expression());
      } while (Match(COMMA));
    }

    Token paren = Eat(RIGHT_PARENTESIS, "Expected ')' after parameters.");

    return new Expression.Call(calle, paren, parameters);
  }

  private Expression.Expr Call()
  {
    Expression.Expr expr = Primary();

    if (expr is Expression.Variable funName)
    {
      switch (funName.name.type)
      {
        case IDENTIFIER:
        case POINT:
        case LINE:
        case SEGMENT:
        case RAY:
        case ARC:
        case CIRCLE:
          if (Match(LEFT_PARENTESIS))
          {
            expr = FinishCall(expr);
          }
          break;
      }
    }

    return expr;
  }

  private Expression.Expr Primary()
  {
    if (Match(FALSE))
    {
      return new Expression.Literal(false);
    }

    if (Match(TRUE))
    {
      return new Expression.Literal(true);
    }

    if (Match(UNDEFINED))
    {
      return new Expression.Undefined();
    }

    if (Match(NUMBER, STRING))
    {
      return new Expression.Literal(Previous().literal);
    }

    if (Match(IDENTIFIER, POINT, LINE, SEGMENT, RAY, ARC, CIRCLE))
    {
      return new Expression.Variable(Previous());
    }

    if (Match(LEFT_PARENTESIS))
    {
      Expression.Expr expr = Expression();
      Eat(RIGHT_PARENTESIS, "Expected ')' after expression.");
      return expr;
    }

    if (Match(LEFT_BRACE))
    {
      return SequenceDeclaration(Previous());
    }

    throw Error(Peek(), "Expected expression.");
  }

  private bool Match(params TokenType[] types)
  {
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

  private Token Eat(TokenType type, string message)
  {
    if (Check(type)) return Advance();
    logger.Error("SYNTAX", Peek(), message);
    return null;
  }

  private bool Check(TokenType type)
  {
    if (IsAtEnd()) return false;
    return Peek().type == type;
  }

  private bool CheckNext(TokenType type)
  {
    if (IsAtEnd()) return false;
    return PeekNext().type == type;
  }

  private bool IsFunction()
  {
    if (Check(IDENTIFIER) && CheckNext(LEFT_PARENTESIS))
    {
      for (int i = current; tokens[i].type != EOF; i++)
      {
        if (tokens[i].type == RIGHT_PARENTESIS)
        {
          return tokens[i + 1].type == EQUAL;
        }
      }
    }
    return false;
  }

  private bool IsConstant()
  {
    if (Check(IDENTIFIER) && CheckNext(LEFT_PARENTESIS))
    {
      return false;
    }

    return Check(IDENTIFIER);
  }

  private Token Advance()
  {
    if (!IsAtEnd()) current++;
    return Previous();
  }

  private bool IsAtEnd()
  {
    return Peek().type == EOF;
  }

  private Token Peek()
  {
    return tokens[current];
  }

  private Token PeekNext()
  {
    return tokens[current + 1];
  }

  private Token Previous()
  {
    return tokens[current - 1];
  }

  private ParseError Error(Token token, string message)
  {
    logger.Error("SYNTAX", token, message);
    return new ParseError();
  }

  private void Synchronize()
  {
    Advance();

    while (!IsAtEnd())
    {
      if (Previous().type == SEMICOLON)
        return;

      switch (Previous().type)
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
}