using System.Collections.Immutable;

namespace GSharp.Exceptions;

public class WarningType
{
  // gets the name of the warning
  public string Name { get; }

  // 'undefined' is used. This warning covers multiples usages of 'undefined', like
  // variable assignment ('s = undefined') and function calls ('foo(undefined)').
  public static readonly WarningType UNDEFINED_USAGE = new("undefined-usage");

  // an ambiguous combination of boolean operators was encountered.
  // this can be thing like 'false && false || true', an expression like this is valid
  // in most languages, and in many of them the '&&' operator has a higher precedence
  // than '||'. However, remember the operator precedence rules by heart is not always
  // easy. We want to help people write code which is unambiguous to the reader.
  public static readonly WarningType AMBIGUOUS_COMBINATION_OF_BOOLEAN_OPERATORS = new("ambiguous-combination-of-boolean-operators");

  public static readonly IReadOnlyDictionary<string, WarningType> AllWarnings = new[]
  {
    UNDEFINED_USAGE
  }.ToImmutableDictionary(w => w.Name, w => w);

  private WarningType(string Name)
  {
    this.Name = Name;
  }

  public static bool KnownWarning(string warningName)
  {
    return AllWarnings.ContainsKey(warningName);
  }

  public static WarningType Get(string warningName)
  {
    return AllWarnings[warningName];
  }
}