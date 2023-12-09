using GSharp.Statement;
using GSharp.Expression;
using GSharp.Exceptions;

namespace GSharp.Interpreter;

/// <summary>
/// A binding to a function defined in GSharp.
/// </summary>
internal class FunctionBinding : Binding, IDistanceBinding
{
  public int Distance { get; }
  public Function Fun { get; }

  public override string ObjectType => "function";

  public FunctionBinding(Function function, ITypeReference typeReference, int distance, Expr referringExpr) : base(typeReference, referringExpr)
  {
    // likewise, the function property is permitted to be null for variable bindings
    if (referringExpr is Call)
    {
      Fun = function ?? throw new InterpreterException("When referringExpr is an Call instance, function cannot be null.");
    }

    this.Distance = distance;
  }
}