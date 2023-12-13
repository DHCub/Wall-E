using GSharp.Statement;
using GSharp.Expression;

namespace GSharp.Interpreter;

internal class FunctionBindingFactory : IBindingFactory
{
  public ITypeReference TypeReference { get; }
  public Function Fun { get; }

  public string ObjectType => "function";

  public FunctionBindingFactory(ITypeReference typeReference, Function function)
  {
    this.TypeReference = typeReference;
    this.Fun = function;
  }

  public Binding CreateBinding(int distance, Expr referringExpr) =>
    new FunctionBinding(Fun, TypeReference, distance, referringExpr);
}