namespace GSharp;

public interface ICallable 
{
  object Call(IInterpreter interpreter, List<object> arguments);
  int Arity();
}