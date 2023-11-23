namespace GSharp;

using System.Linq.Expressions;
using System.Reflection.Emit;
using static TokenType;
using System;
using System.Collections.Generic;

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

  public List<Stmt> Parse()
  {
    List<Stmt> result = new List<Stmt>();
    while (!IsAtEnd())
    {
      result.Add(Declaration());
    }
    return result;
  }

  private Expr Expression()
  {
    if (Match(IF)) return IfExpression();
    if (Match(LET)) return LetInExpression();
    // if (Match(LEFT_BRACE)) return SequenceDeclaration();
    return Assignment();
  }

  private Stmt Declaration()
  {
    try
    {
      if (IsFunction())
      {
        return FunDeclaration();
      }

      if (Match(POINT, LINE, SEGMENT, RAY, CIRCLE))
      {
        return VarDeclaration(Previous());
      }

      if (Match(IDENTIFIER))
      {
        return ConstDeclaration();
      }

      if (Match(DRAW))
      {
        return DrawCommand();
      }

      if (Match(COLOR))
      {
        return ColorCommand();
      }

      if (Match(RESTORE))
      {
        return RestoreCommand();
      }

      if (Match(IMPORT))
      {
        return ImportLibrary();
      }

      return Statement();
    }
    catch
    {
      Synchronize();
      return null;
    }
  }

  private Stmt Statement()
  {
    return ExpressionStatement();
  }

  private Stmt ExpressionStatement()
  {
    Expr expr = Expression();

    return new Expression(expr);
  }

  private Expr IfExpression()
  {
    Expr condition = Expression();
    Eat(THEN, "Expected if <expr> then <expr> else <expr>.");

    Expr thenBranch = Expression();
    Eat(ELSE, "Expected if <expr> then <expr> else <expr>.");
    Expr elseBranch = Expression();
    return new Conditional(condition, thenBranch, elseBranch);
  }

  private Stmt FunDeclaration()
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
    Expr body = Expression();
    Eat(SEMICOLON, "Expected ';' after function declaration.");
    return new Function(name, parameters, body);
  }

  private Stmt DrawCommand()
  {
    Expr elements = Expression();

    Token nameId = null;
    if (Check(STRING))
    {
      nameId = Eat(STRING, "Expected string.");
    }

    Eat(SEMICOLON, "Expected 'l' after draw command.");
    return new Draw(elements, nameId);
  }

  private Stmt ColorCommand()
  {
    if (Match(BLUE, RED, YELLOW, GREEN, CYAN, MAGENTA, WHITE, GRAY, BLACK))
    {
      Token color = Previous();
      Eat(SEMICOLON, "Expected ';' after color command.");
      return new Color(color);
    }
    else
    {
      Error(Peek(), "Expected a valid color.");
      return null;
    }
  }

  private Stmt RestoreCommand()
  {
    Eat(SEMICOLON, "Expected ';' after restore command.");
    return null;
  }

  private Stmt ImportLibrary()
  {
    Token dirName = Eat(STRING, "Expected directory name.");
    return new Import(dirName);
  }

  private Stmt VarDeclaration(Token type)
  {
    bool isSequence = false;
    if (Match(SEQUENCE))
    {
      isSequence = true;
    }

    Token name = Eat(IDENTIFIER, "Expected variable name.");

    Stmt initializer = null;

    Eat(SEMICOLON, "Expected ';' after variable declaration.");
    return new Var(type, name, initializer, isSequence);
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

  private Stmt ConstDeclaration()
  {
    List<Token> constNames = GetNames();
    Eat(EQUAL, "Expected '=' after constant name.");
    Expr initializer = Expression();
    Eat(SEMICOLON, "Expected ';' after constant declaration.");
    return new Constant(constNames, initializer);
  }

  private List<Assign> Instructions()
  {
    List<Assign> instructions = new List<Assign>();
    while (!Check(IN))
    {
      if (Check(IN)) break;

      if (!Match(IDENTIFIER))
      {
        Error(Peek(), "Invalid token in 'let-in' expression.");
        return null;
      }

      List<Token> names = GetNames();
      Eat(EQUAL, "Expected '=' after variable.");
      Expr value = Expression();
      Eat(SEMICOLON, "Expected ';' after instruction.");
      instructions.Add(new Assign(names, value));
    }
    return instructions;
  }

  private Expr SequenceDeclaration()
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

        if (CheckNext(DOTS))
        {
          Token left = Eat(NUMBER, "Range limit must be a integer constant.");
          Eat(DOTS, "Expected '...' after left limit of range.");

          Token right = null;
          if (Check(NUMBER))
            right = Eat(NUMBER, "Range limit must be a integer constant.");

          items.Add(new Range(left, right));
        }
        else
        {
          items.Add(Expression());
        }
      } while (Match(COMMA));
    }

    Eat(RIGHT_BRACE, "Expected '}' after sequence declaration.");

    return new Sequence(items);
  }

  private Expr LetInExpression()
  {
    var instructions = Instructions();
    Eat(IN, "Expected 'in' at end of 'let-in' expression.");
    Expr body = Expression();
    return new LetIn(instructions, body);
  }

  private Expr Assignment()
  {
    Expr expr = Or();
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
    Expr expr = Unary();
    while (Match(DIV, MUL))
    {
      Token oper = Previous();
      Expr right = Unary();
      expr = new Binary(expr, oper, right);
    }
    return expr;
  }

  private Expr Unary()
  {
    if (Match(NOT, MINUS))
    {
      Token oper = Previous();
      Expr right = Unary();
      return new Unary(oper, right);
    }

    return Call();
  }

  private Expr FinishCall(Expr calle)
  {
    List<Expr> parameters = new List<Expr>();
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

    Eat(RIGHT_PARENTESIS, "Expected ')' after parameters.");

    return new Call(calle, parameters);
  }

  private Expr Call()
  {
    Expr expr = Primary();

    if (expr is Variable funName) {
      if (funName.name.type == IDENTIFIER) {
        if (Match(LEFT_PARENTESIS))
        {
          expr = FinishCall(expr);
        }
      }
    }
    
    return expr;
  }

  private Expr Primary()
  {
    if (Match(FALSE))
    {
      return new Literal(false);
    }

    if (Match(TRUE))
    {
      return new Literal(TRUE);
    }

    if (Match(UNDEFINED))
    {
      return new Undefined();
    }

    if (Match(NUMBER, STRING))
    {
      return new Literal(Previous().literal);
    }

    if (Match(IDENTIFIER, LINE, SEGMENT, RAY, ARC, CIRCLE))
    {
      return new Variable(Previous());
    }

    if (Match(LEFT_PARENTESIS))
    {
      Expr expr = Expression();
      Eat(RIGHT_PARENTESIS, "Expected ')' after expression.");
      return expr;
    }

    if (Match(LEFT_BRACE))
    {
      return SequenceDeclaration();
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
    return Check(IDENTIFIER) && CheckNext(LEFT_PARENTESIS);
  }

  private Token Advance()
  {
    if (!IsAtEnd()) current++;
    return Previous();
  }

  private bool IsAtEnd()
  {
    return Peek().type == TokenType.EOF;
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