using System;

namespace GSharp;

internal readonly struct FloatingPointLiteral<T> : IFloatingPointLiteral, INumericLiteral
  where T : notnull
{
  public object value { get; }
  public string number { get; }
  public bool isPositive { get; }

  public FloatingPointLiteral(T value, string number)
  {
    this.value = value;
    this.number = number;

    isPositive = value switch
    {
      float floatValue => floatValue >= 0,
      double doubleValue => doubleValue >= 0,
      _ => throw new ArgumentException($"Unsupported numeric type encountered: {value.GetType().Name}")
    };
  }
  public override string? ToString()
  {
    return value.ToString();
  }
}