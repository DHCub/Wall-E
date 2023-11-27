using System.Collections.Generic;
using GSharp;

public class Environment
{

  public readonly Environment enclosing;
  private readonly Dictionary<string, object> values;

  Environment()
  {
    this.enclosing = null;
    this.values = new Dictionary<string, object>();
  }

  public object Get(Token name)
  {
    if (values.ContainsKey(name.lexeme))
    {
      return values[name.lexeme];
    }

    if (enclosing is not null)
      return enclosing.Get(name);

    throw new RuntimeError(name.lexeme, "Undefined variable.");
  }

  public void Assign(Token name, object value)
  {
    if (values.ContainsKey(name.lexeme))
    {
      values[name.lexeme] = value;
      return;
    }

    if (enclosing is not null)
    {
      enclosing.Assign(name, value);
      return;
    }

    throw new RuntimeError(name.lexeme, "Undefined variable.");
  }

  public void Define(string name, object value)
  {
    values.Add(name, value);
  }

  public Environment Ancestor(int distance)
  {
    Environment environment = this;
    for (int i = 0; i < distance; i++)
    {
      environment = environment.enclosing;
    }
    return environment;
  }

  public object GetAt(int distance, string name)
  {
    return Ancestor(distance).values[name];
  }

  public void AssignAt(int distance, Token name, object value)
  {
    Ancestor(distance).values.Add(name.lexeme, value);
  }

  public override string ToString()
  {
    string result = values.ToString();
    if (enclosing != null)
    {
      result += " -> " + enclosing.ToString();
    }
    return result;
  }

}