using GSharp.Expression;
using GSharp.Statement;

namespace GSharp.Interpreter;

public interface IBindingHandler
{
  bool GetLocalBinding(Expr expr, out Binding? binding);
  Binding? GetVariableOrFunctionBinding(Expr expr);

  // adds an expression to the global scope
  void AddGlobalExpr(Binding binding);

  // adds an expression to a local scope at a given depth away from the call site
  void AddLocalExpr(Binding binding);
}