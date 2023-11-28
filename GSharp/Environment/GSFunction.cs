namespace GSharp;

using System.Collections.Generic;
using GSharp.Core;
using GSharp.Statement;

public class GSFunction : ICallable
{
  private readonly Function declaration;
  private readonly Environment closure;
  private readonly bool isInitializer;

  public GSFunction(Function declaration, Environment closure, bool isInitializer)
  {
    this.isInitializer = isInitializer;
    this.closure = closure;
    this.declaration = declaration;
  }
  public override string ToString()
  {
    return "<fun " + declaration.name.lexeme + ">";
  }

  public int Arity() {
    return declaration.parameters.Count;
  }

  public object Call(Interpreter interpreter, List<object> arguments)
  {
    Environment environment = new Environment(closure);

    for (int i = 0; i < declaration.parameters.Count; i++)
    {
      environment.Define(declaration.parameters[i].lexeme, arguments[i]);
    }

    return null;
  }

}