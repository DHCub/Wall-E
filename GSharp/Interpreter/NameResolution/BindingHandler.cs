using GSharp.Statement;
using GSharp.Expression;

namespace GSharp.Interpreter;

// <summary>
// Class responsible for handling global and local bindings.
// </summary>

internal class BindingHandler : IBindingHandler
{
  // map from referring expression to global binding (var or fun)
  private readonly IDictionary<Expr, Binding> globalBindings = new Dictionary<Expr, Binding>();

  // map from referring expression to local binding (in a local scope) for var or fun
  private readonly IDictionary<Expr, Binding> localBindings = new Dictionary<Expr, Binding>();

  public void AddGlobalExpr(Binding binding)
  {
    globalBindings[binding.ReferringExpr] = binding;
  }

  public void AddLocalExpr(Binding binding)
  {
    localBindings[binding.ReferringExpr] = binding;
  }

  public bool GetLocalBinding(Expr expr, out Binding? binding)
  {
    return localBindings.TryGetValue(expr, out binding);
  }

  public Binding? GetVariableOrFunctionBinding(Expr expr)
  {
    if (localBindings.ContainsKey(expr))
    {
      return localBindings[expr];
    }

    if (globalBindings.ContainsKey(expr))
    {
      return globalBindings[expr];
    }

    // the variable does not exist
    return null;
  }
}