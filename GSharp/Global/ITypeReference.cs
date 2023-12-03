using System;
using System.Numerics;

namespace GSharp;

public interface ITypeReference
{
  Token? TypeSpecifier { get; }

  // gets or sets the csharp type that this ITypeReference refers to
  Type? CSharpType { get; set; }

  // gets the gsharp type that this ITypeReference refers to
  string GSharpType { get; }

  // gets a value indicating whether the type reference contains an
  // explicit type specifier or not. If this is fase, the user perhaps
  // intending for the type to be inferred from the program context.
  bool ExplicitTypeSpecified => TypeSpecifier != null;

  // gets a value indicating whether 
  bool IsResolved => CSharpType != null;

  // gets a value indicating whether this type reference refers to a undefined value
  public bool IsUndefinedObject => CSharpType == typeof(Objects.Undefined);

  bool IsValidNumberType =>
    CSharpType switch
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
    CSharpType switch
    {
      null => false,
      var t when t == typeof(string) => true,
      _ => false
    };
}