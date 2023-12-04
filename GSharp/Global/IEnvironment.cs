using GSharp.Objects;

namespace GSharp;

public interface IEnvironment
{
  void Define(Token name, GSObject value);
  GSObject GetAt(int distance, string name);
  void AssignAt(int distance, Token name, GSObject value);
}