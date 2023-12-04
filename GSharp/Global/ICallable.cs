using GSharp.Objects;

namespace GSharp;

public interface ICallable
{
  GSObject Call(IInterpreter interpreter, List<GSObject> arguments);
  int Arity();
}