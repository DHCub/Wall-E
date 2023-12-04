using System;
using GSharp.Types;

namespace GSharp;

public class Parameter
{
  public Token Name { get; }
  public ITypeReference TypeReference { get; }

  public Token TypeSpecifier => TypeReference.TypeSpecifier;

  public readonly TypeName? typeName;

  // public Parameter(ITypeReference TypeReference, TypeName? typeName = null)
  // {
  //   this.typeName = typeName;
  //   this.TypeReference = TypeReference;
  // }

  public Parameter(Token Name, ITypeReference TypeReference, TypeName? typeName)
  {
    this.Name = Name;
    this.TypeReference = TypeReference;
    this.typeName = typeName;
  }
}