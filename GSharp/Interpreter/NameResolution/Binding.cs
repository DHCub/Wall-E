using GSharp.Exceptions;
using GSharp.Expression;

namespace GSharp.Interpreter;

/// <summary>
/// Holds information about a binding
///
/// Different types of bindings provide slightly different mechanisms to retrieve
/// information about the variable or function being bound to.
/// </summary>
public abstract class Binding
{
  // gets the type reference of the declaring statement (typically a 'var' initializer or a function return type)
  public ITypeReference? TypeReference { get; }

  public Expr ReferringExpr { get; }

  // gets a value indicating whether this binding is mutable or immutable.
  public virtual bool IsMutable => false;
  public bool IsImmutable => !IsMutable;

  public abstract string ObjectType { get; }

  // gets the type of object this binding refers to, with the initial letter converted to upper-case
  public object ObjectTypeTitleized => ObjectType[0].ToString().ToUpper() + ObjectType.Substring(1);

  protected Binding(ITypeReference? typeReference, Expr referringExpr)
  {
    this.TypeReference = typeReference;
    this.ReferringExpr = referringExpr ?? throw new InterpreterException("referringExpr cannot be null.");
  }

  public override string ToString()
  {
    return $"{GetType().Name}" +
           "{" +
           $"{nameof(ReferringExpr)}: {ReferringExpr}, " +
           $"{nameof(TypeReference)}: {TypeReference}, " +
           $"{nameof(IsImmutable)}: {IsImmutable}, " +
           $"{nameof(ObjectType)}: {ObjectType}, " +
           $"GetHashCode: {GetHashCode()}" +
           "}";
  }

}