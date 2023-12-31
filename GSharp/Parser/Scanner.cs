using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GSharp.Exceptions;

namespace GSharp.Parser;
using static TokenType;

/// <summary>
/// Scans a GSharp program, converting it to a list of Tokens.
/// </summary>
public class Scanner
{
  public static readonly Dictionary<string, TokenType> ReservedKeywords = new Dictionary<string, TokenType>
  {
    {"point", POINT},
    {"line", LINE},
    {"ray", RAY},
    {"segment", SEGMENT},
    {"circle", CIRCLE},
    {"arc", ARC},
    {"sequence", SEQUENCE},

    {"if", IF},
    {"then", THEN},
    {"else", ELSE},
    {"false", FALSE},
    {"true", TRUE},
    {"while", WHILE},
    {"for", FOR},

    {"and", AND},
    {"or", OR},
    {"not", NOT},

    {"let", LET},
    {"in", IN},

    {"import", IMPORT},
    {"draw", DRAW},
    {"color", COLOR},
    {"restore", RESTORE},
    {"print", PRINT},

    {"red", RED},
    {"green", GREEN},
    {"blue", BLUE},
    {"gray", GRAY},
    {"magenta", MAGENTA},
    {"cyan", CYAN},
    {"yellow", YELLOW},
    {"orange", ORANGE},
    {"white", WHITE},
    {"black", BLACK},

    {"undefined", UNDEFINED}
  };

  public static IEnumerable<string> ReservedTypeKeywordStrings =>
    new List<string>
    {
      "measure",
      "intersect",
      "count",
      "randoms",
      "points",
      "samples"
    }.ToImmutableHashSet();

  private static ISet<string> reservedKeywordOnlyStrings;

  public static ISet<string> ReservedKeywordOnlyStrings
  {
    get
    {
      reservedKeywordOnlyStrings ??= ReservedKeywords.Select(kvp => kvp.Key).ToImmutableHashSet();

      return reservedKeywordOnlyStrings;
    }
  }

  public static IEnumerable<string> ReservedKeywordStrings =>
    ReservedKeywordOnlyStrings.Concat(ReservedTypeKeywordStrings).ToImmutableHashSet();

  public readonly string source;
  private readonly ScanErrorHandler scanErrorHandler;

  private readonly List<Token> tokens = new();
  private int start;
  private int current;
  private int line = 1;
  private Stack<string> importTrace;

  public Scanner(string source, ScanErrorHandler scanErrorHandler, Stack<string> importTrace)
  {
    this.source = source;
    this.scanErrorHandler = scanErrorHandler;
    this.importTrace = importTrace;
  }

  public List<Token> ScanTokens()
  {
    while (!IsAtEnd())
    {
      // We are at the beginning of the next lexeme
      start = current;
      ScanToken();
    }

    tokens.Add(new Token(EOF, "", null, line, current));
    return tokens;
  }

  private void ScanToken()
  {
    char c = Advance();
    switch (c)
    {
      case ' ':
      case '\r':
      case '\t':
        // ignore whitespaces
        break;

      case '\n':
        // new lines are tracked to increase the newline count
        line++;
        break;

      case '"':
        ScanString();
        break;

      case '!':
        AddToken(Match('=') ? NOT_EQUAL : NOT);
        break;

      case '+':
        if (Match('+'))
        {
          AddToken(PLUS_PLUS);
        }
        else if (Match('='))
        {
          AddToken(PLUS_EQUAL);
        }
        else
        {
          AddToken(PLUS);
        }
        break;
      case '-':
        if (Match('-'))
        {
          AddToken(MINUS_MINUS);
        }
        else if (Match('='))
        {
          AddToken(MINUS_EQUAL);
        }
        else
        {
          AddToken(MINUS);
        }
        break;
      case '%':
        AddToken(MOD);
        break;
      case '/':
        if (Match('/'))
        {
          // a comment goes until the end of the line.
          while (Peek() != '\n' && !IsAtEnd()) Advance();
        }
        else
        {
          AddToken(DIV);
        }
        break;
      case '*':
        AddToken(MUL);
        break;
      case '^':
        AddToken(POWER);
        break;

      case '|':
        AddToken(OR);
        break;
      case '&':
        AddToken(AND);
        break;

      case ',':
        AddToken(COMMA);
        break;
      case ';':
        AddToken(SEMICOLON);
        break;
      case ':':
        AddToken(Match('=') ? ASSIGN : TWO_DOTS);
        break;

      case '(':
        AddToken(LEFT_PARENTESIS);
        break;
      case ')':
        AddToken(RIGHT_PARENTESIS);
        break;
      case '{':
        AddToken(LEFT_BRACE);
        break;
      case '}':
        AddToken(RIGHT_BRACE);
        break;
      case '[':
        AddToken(LEFT_SQUARE_BRACKET);
        break;
      case ']':
        AddToken(RIGHT_SQUARE_BRACKET);
        break;

      case '=':
        AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
        break;
      case '<':
        AddToken(Match('=') ? LESS_EQUAL : LESS);
        break;
      case '>':
        AddToken(Match('=') ? GREATER_EQUAL : GREATER);
        break;
      case '.':
        bool errorFound = false;
        if (Match('.') && Match('.'))
        {
          AddToken(DOTS);
        }
        else
        {
          errorFound = true;
        }

        if (errorFound)
        {
          // unexpected character
          scanErrorHandler(new ScanError("Unexpected character.", line, importTrace));
        }

        break;

      default:
        if (IsDigit(c))
        {
          ScanNumber();
        }
        else if (IsAlpha(c) || IsUnderscore(c))
        {
          ScanIdentifier();
        }
        else
        {
          // unexpected character
          scanErrorHandler(new ScanError("Unexpected character." + c, line, importTrace));
        }
        break;
    }
  }

  private void ScanIdentifier()
  {
    while (IsAlphaNumeric(Peek())) Advance();

    // see if the identifier is a reserved word
    string text = source[start..current];

    var type = ReservedKeywords.ContainsKey(text) ? ReservedKeywords[text] : IDENTIFIER;
    AddToken(type);
  }

  private void ScanNumber()
  {
    bool isFractional = false;

    while (IsDigit(Peek())) Advance();

    // look for a fractional part
    if (Peek() == '.' && IsDigit(PeekNext()))
    {
      isFractional = true;
      Advance();
      while (IsDigit(Peek())) Advance();
    }

    if (IsAlpha(Peek()) || IsUnderscore(Peek()))
    {
      Advance();
      while (IsAlphaNumeric(Peek())) Advance();
      string text = source[start..current];
      scanErrorHandler(new ScanError($"{text} is not a valid token", line, importTrace));
      return;
    }

    AddToken(new NumericToken(source[start..current], source[start..current], line, current, isFractional));
  }

  private void ScanString()
  {
    while (Peek() != '"' && !IsAtEnd())
    {
      if (Peek() == '\\' && PeekNext() == '"')
      {
        Advance();
      }
      if (Peek() == '\n') line++;
      Advance();
    }

    // unterminated string
    if (IsAtEnd())
    {
      scanErrorHandler(new ScanError("Unterminated string.", line, importTrace));
      return;
    }

    // closing "
    Advance();

    // trim the surrounding quotes
    string value = source[(start + 1)..(current - 1)];

    value = value.Replace("\\t", "\t").Replace("\\n", "\n").Replace("\\\"", "\"");

    AddToken(STRING, value);
  }

  private bool Match(char expected)
  {
    if (IsAtEnd()) return false;
    if (source[current] == expected)
    {
      Advance();
      return true;
    }
    return false;
  }

  private char Peek()
  {
    if (IsAtEnd()) return '\0';
    return source[current];
  }

  private char PeekNext()
  {
    if (current + 1 >= source.Count()) return '\0';
    return source[current + 1];
  }

  private bool IsAlpha(char c)
  {
    return (c is >= 'a' and <= 'z') ||
           (c is >= 'A' and <= 'Z');
  }

  private bool IsUnderscore(char c)
  {
    return c is '_';
  }

  private bool IsAlphaNumeric(char c)
  {
    return IsAlpha(c) || IsUnderscore(c) || IsDigit(c);
  }

  private bool IsDigit(char c)
  {
    return c is >= '0' and <= '9';
  }

  private bool IsAtEnd()
  {
    return current >= source.Length;
  }

  private char Advance()
  {
    return source[current++];
  }

  private void AddToken(Token token)
  {
    tokens.Add(token);
  }

  private void AddToken(TokenType type, object literal = null)
  {
    string text = source.Substring(start, current - start);

    Token newToken = new Token(type, text, literal, line, current);
    if (type == SEQUENCE)
    {
      if (tokens.Count > 0)
      {
        switch (tokens.Last().type)
        {
          case POINT:
            tokens.RemoveAt(tokens.Count - 1);
            newToken = new Token(POINT_SEQUENCE, "point sequence", literal, line, current);
            tokens.Add(newToken);
            return;
          case LINE:
            tokens.RemoveAt(tokens.Count - 1);
            newToken = new Token(LINE_SEQUENCE, "line sequence", literal, line, current);
            tokens.Add(newToken);
            return;
          case SEGMENT:
            tokens.RemoveAt(tokens.Count - 1);
            newToken = new Token(SEGMENT_SEQUENCE, "segment sequence", literal, line, current);
            tokens.Add(newToken);
            return;
          case RAY:
            tokens.RemoveAt(tokens.Count - 1);
            newToken = new Token(RAY_SEQUENCE, "ray sequence", literal, line, current);
            tokens.Add(newToken);
            return;
          case ARC:
            tokens.RemoveAt(tokens.Count - 1);
            newToken = new Token(ARC_SEQUENCE, "arc sequence", literal, line, current);
            tokens.Add(newToken);
            return;
          case CIRCLE:
            tokens.RemoveAt(tokens.Count - 1);
            newToken = new Token(CIRCLE_SEQUENCE, "circle sequence", literal, line, current);
            tokens.Add(newToken);
            return;
          default:
            break;
        }
      }
    }

    tokens.Add(newToken);
  }
}