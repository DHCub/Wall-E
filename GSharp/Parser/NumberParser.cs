using System;
using System.Globalization;
using System.Numerics;

namespace GSharp;

internal static class NumberParser
{
  public static INumericLiteral Parse(NumericToken numericToken)
  {
    string number = (string)numericToken.literal!;

    // use 'double' precision by default
    double value = double.Parse(number, CultureInfo.InvariantCulture);
    return new FloatingPointLiteral<double>(value, number);
  }

  public static object MakeNegative(INumericLiteral numericLiteral)
  {
    if (numericLiteral is IFloatingPointLiteral floatingPointLiteral)
    {
      if (numericLiteral.value is float floatValue)
      {
        return new FloatingPointLiteral<float>(-floatValue, "-" + floatingPointLiteral.number);
      }
      else if (numericLiteral.value is double doubleValue)
      {
        return new FloatingPointLiteral<double>(-doubleValue, "-" + floatingPointLiteral.number);
      }
    }

    throw new ArgumentException($"Type {numericLiteral.value.GetType().Name} not supported.");
  }

}