namespace GSharp;

using System.Collections.Generic;

interface ICallable
{
  public int Arity();

  public object Call(Core.Interpreter interpreter, List<object> arguments);
}