#nullable enable

namespace GSharp;

public interface INumericLiteral
{
  public object value { get; }
  public bool isPositive { get; }
}