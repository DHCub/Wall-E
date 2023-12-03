namespace GSharp;

public enum TokenType
{

  // Single-character Tokens
  LEFT_BRACE, RIGHT_BRACE, LEFT_PARENTESIS, RIGHT_PARENTESIS, COMMA, DOTS, TWO_DOTS, SEMICOLON,

  // Comparison Tokens
  NOT, NOT_EQUAL, EQUAL, EQUAL_EQUAL, GREATER, GREATER_EQUAL, LESS, LESS_EQUAL,

  // Operators
  MUL, MINUS, PLUS, POWER, DIV, MOD,

  // Literals
  STRING, NUMBER, IDENTIFIER, UNDEFINED,

  // Keywords
  IMPORT, AND, ELSE, FALSE, THEN, IF, LET, IN, OR, TRUE, PRINT,

  // Geometry elements
  POINT, LINE, SEGMENT, RAY, ARC, CIRCLE, SEQUENCE,

  // Sequence
  POINT_SEQUENCE, LINE_SEQUENCE, SEGMENT_SEQUENCE, RAY_SEQUENCE, ARC_SEQUENCE, CIRCLE_SEQUENCE,

  // Graphics elements
  DRAW, COLOR, RESTORE, RED, GREEN, BLUE, GRAY, MAGENTA, CYAN, YELLOW, ORANGE, WHITE, BLACK,

  EOF
}