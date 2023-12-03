using GSharp.Statement;

namespace GSharp;

public interface IInterpreter<T>
{
  void ExecuteBlock(IEnumerable<Stmt> statements, IEnvironment<T> blockEnvironment);
}