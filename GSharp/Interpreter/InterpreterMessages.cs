namespace GSharp.Interpreter;

public static class InterpreterMessages
{
  public static string UnsupportedOperandTypes(TokenType operatorType, object left, object right) =>
       $"Unexpected runtime error: Unsupported {operatorType} operands specified: {left} and {right}";

  public static string UnsupportedOperatorTypeInBinaryExpression(TokenType operatorType) =>
      $"Internal error: Unsupported operator {operatorType} in binary expression.";
}