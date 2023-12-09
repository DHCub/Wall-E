using GSharp.Expression;

namespace GSharp.Interpreter;

/// <summary>
/// Container class which supports creating VariableBinding instances.
/// </summary>
internal class VariableBindingFactory : IBindingFactory
{
  public static VariableBindingFactory None { get; } = new VariableBindingFactory(null);

  public string ObjectType => "variable";
  public ITypeReference TypeReference { get; }

  public VariableBindingFactory(ITypeReference typeReference)
  {
    this.TypeReference = typeReference;
  }

  public Binding CreateBinding(int distance, Expr referringExpr)
  {
    return new VariableBinding(TypeReference, distance, referringExpr);
  }
}