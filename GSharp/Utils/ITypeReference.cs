using System;
using System.Numerics;

namespace GSharp;

public interface ITypeReference
{
  Token? typeSpecifier { get; }

  // gets or sets the csharp type that this ITypeReference refers to
  Type? csType { get; set; }

  // gets the gsharp type that this ITypeReference refers to
  string infType { get; }

  // gets a value indicating whether the type reference contains an
  // explicit type specifier or not. If this is fase, the user perhaps
  // intending for the type to be inferred from the program context.
  bool ExplicitTypeSpecified => typeSpecifier != null;

  // gets a value indicating whether 
  bool IsResolved => csType != null;

  // gets a value indicating whether this type reference refers to a 'null' value
  public bool IsUndefinedObject => csType == typeof(NullObject);

  bool IsValidNumberType =>
    csType switch
    {
      null => false,
      var t when t == typeof(sbyte) => true,
      var t when t == typeof(short) => true,
      var t when t == typeof(int) => true,
      var t when t == typeof(long) => true,
      var t when t == typeof(byte) => true,
      var t when t == typeof(ushort) => true,
      var t when t == typeof(uint) => true,
      var t when t == typeof(ulong) => true,
      var t when t == typeof(float) => true,
      var t when t == typeof(double) => true,
      var t when t == typeof(BigInteger) => true,
      _ => false
    };

  bool IsStringType() =>
    csType switch
    {
      null => false,
      var t when t == typeof(string) => true,
      _ => false
    };
}