using GSharp.Statement;
using GSharp.Expression;
using GSharp.Objects;
using System.Collections.Generic;

namespace GSharp;

public interface IInterpreter
{
  void ExecuteBlock(IEnumerable<Stmt>? statements, IEnvironment blockEnvironment);
}