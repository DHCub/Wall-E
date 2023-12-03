using System;
using System.Reflection;

namespace GSharp;

public class TypeReference : ITypeReference
{
  private Type? CsType;
  public Token? typeSpecifier { get; }

  public Type? csType {
    get => csType;
    set
    {
      if (CsType is not null && CsType != value) {
        throw new ArgumentException($"CSType already set to {CsType}; property is read-only. Attempted to set it to {value}.");
      }

      CsType = value;
    }
  }

  public string infType => 
    CsType switch
    {
      var t when t == typeof(Geometry.Point) => "Point",
      var t when t == typeof(Geometry.Segment) => "Segment",
      var t when t == typeof(Geometry.Line) => "Line",
      _ => throw new Exception("")
    };
}