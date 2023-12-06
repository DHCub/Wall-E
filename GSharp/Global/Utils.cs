using System;

namespace GSharp.Numeric;

public static class Numerical
{
  public static decimal Sqrt(decimal value)
  {
    if (value < 0)
    {
      throw new OverflowException("Cannot calculate square root from a negative number");
    }

    decimal lo = 0, hi = (decimal)Math.Sqrt((double)value);
    for (int round = 0; round < 128; round++)
    {
      decimal mid = (lo + hi) / 2;
      if (mid * mid <= value)
      {
        lo = mid;
      }
      else
      {
        hi = mid;
      }
    }
    return lo;
  }

  public static decimal Pow(decimal b, decimal e)
  {
    decimal BinaryPow(decimal b, decimal e)
    {
      if (e == 0) return 1;
      if (e % 2 == 1)
      {
        return BinaryPow(b, e - 1) * b;
      }
      return BinaryPow(b, e / 2) * BinaryPow(b, e / 2);
    }

    if (e >= 0)
    {
      return BinaryPow(b, e);
    }
    else
    {
      return (decimal)1.0 / BinaryPow(b, -e);
    }
  }
}