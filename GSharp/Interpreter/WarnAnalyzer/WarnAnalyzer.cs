
using System;
using System.Collections.Immutable;
using GSharp;
using GSharp.Exceptions;
using GSharp.Expression;
using GSharp.Statement;
using static GSharp.TokenType;

/// <summary>
/// Validator which performs various forms of warning-analysis related validation.
/// 
/// One example of such validation is "valid combination of expressions". Sometimes,
/// expressions are combined in a way which is "semantically permitted" by our parser,
/// but the result is still "logically forbidden" or discouraged because undesired ambiguity
/// that it forces upon the reader of the code.
/// </summary>
internal class WarnAnalyzer : VisitorBase
{
  private readonly Action<CompilerWarning> compilerWarningCallback;

  private WarnAnalyzer(Action<CompilerWarning> compilerWarningCallback)
  {
    this.compilerWarningCallback = compilerWarningCallback;
  }

  public static void Validate(ImmutableList<Stmt> statements, Action<CompilerWarning> compilerWarningCallback)
  {
    new WarnAnalyzer(compilerWarningCallback).Visit(statements);
  }

  public override VoidObject VisitLogicalExpr(Logical expr)
  {
    base.VisitLogicalExpr(expr);

    if (expr.Oper.type is OR or AND)
    {
      if (expr.Left is Logical leftLogical && leftLogical.Oper.type != expr.Oper.type)
      {
        compilerWarningCallback(new CompilerWarning($"Invalid combination of boolean operators: {leftLogical.Oper.lexeme} and {expr.Oper.lexeme}. To avoid ambiguity for the reader, grouping parentheses () mus be used.", expr.Token, WarningType.AMBIGUOUS_COMBINATION_OF_BOOLEAN_OPERATORS));
      }
      else if (expr.Right is Logical rightLogical && rightLogical.Oper.type != expr.Oper.type)
      {
        compilerWarningCallback(new CompilerWarning($"Invalid combination of boolean operators: {rightLogical.Oper.lexeme} and {expr.Oper.lexeme}. To avoid ambiguity for the reader, grouping parentheses () mus be used.", expr.Token, WarningType.AMBIGUOUS_COMBINATION_OF_BOOLEAN_OPERATORS));
      }
    }

    return VoidObject.Void;
  }

}