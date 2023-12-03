namespace GSharp;

public interface IEnvironment<T>
{
  void Define(Token name, T value);
  T GetAt(int distance, string name);
  void AssignAt(int distance, Token name, T value);
}