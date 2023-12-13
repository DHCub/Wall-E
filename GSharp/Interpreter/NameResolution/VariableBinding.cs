using GSharp.Expression;

namespace GSharp.Interpreter;

internal class VariableBinding : Binding, IDistanceBinding
{
  public int Distance { get; }

  public override string ObjectType => "variable";
  public override bool IsMutable => true;

  public VariableBinding(ITypeReference? typeReference, int distance, Expr referringExpr) : base(typeReference, referringExpr)
  {
    this.Distance = distance;
  }
}