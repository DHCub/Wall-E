using GSharp.Statement;
using GSharp.Expression;
using GSharp.Objects;

namespace GSharp;

public interface IInterpreter
{
  void ExecuteBlock(IEnumerable<Stmt>? statements, IEnvironment blockEnvironment);
}