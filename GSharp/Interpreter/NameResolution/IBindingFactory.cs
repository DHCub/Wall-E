using GSharp.Expression;

namespace GSharp.Interpreter;

internal interface IBindingFactory
{
  // creates a binding using the provided parameters
  Binding CreateBinding(int distance, Expr referringExpr);

  // gets the type of object this binding refers to.
  string ObjectType { get; }

  // gets the type of object this binding refers to, with the initial letter converted to upper-case
  object ObjectTypeTitleized => ObjectType[0].ToString().ToUpper() + ObjectType.Substring(1);
}