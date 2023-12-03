using System;

namespace GSharp;

public class TypeReference : ITypeReference
{
  private Type? CSType;
  public Token? TypeSpecifier { get; }

  public Type? CSharpType
  {
    get => CSType;
    set
    {
      if (CSType is not null && CSType != value)
      {
        throw new ArgumentException($"CSType already set to {CSType}; property is read-only. Attempted to set it to {value}.");
      }

      CSType = value;
    }
  }

  public string GSharpType =>
    CSType switch
    {
      var t when t == typeof(Objects.Measure) => "Measure",
      var t when t == typeof(Objects.Scalar) => "Scalar",
      var t when t == typeof(Objects.String) => "String",
      var t when t == typeof(Objects.Undefined) => "Undefined",
      var t when t == typeof(Objects.Collections.FiniteStaticSequence) => "FSequence",
      var t when t == typeof(Objects.Collections.GeneratorSequence) => "GSequence",
      var t when t == typeof(Objects.Collections.InfiniteStaticSequence) => "ISequence",
      _ => throw new Exception("")
    };

  public TypeReference(Token? TypeSpecifier)
  {
    // initializes a new instance of the TypeReference class for a given specifier
    // the type specifier can be null, in which case type inference will be attempted.

    this.TypeSpecifier = TypeSpecifier;
  }

  public TypeReference(Type CSType)
  {
    this.CSType = CSType ?? throw new ArgumentException("CSType cannot be null");
  }

  public override string ToString()
  {
    var typeReference = (ITypeReference)this;

    if (typeReference.ExplicitTypeSpecified)
    {
      return typeReference.IsResolved ? $"Explicit: {CSType}" : $"Explicit: {TypeSpecifier}";
    }

    return typeReference.IsResolved ? $"Inferred: {CSType}" : "Inferred, not yet resolved";
  }

}