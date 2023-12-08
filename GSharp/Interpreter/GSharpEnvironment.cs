using GSharp.Exceptions;
using GSharp.Objects;
using System.Collections.Generic;

namespace GSharp.Interpreter;

// <summary>
// Holds information about the context for a static scope (functions and variable name binding).
// </summary>
internal class GSharpEnvironment : IEnvironment
{
  private readonly GSharpEnvironment enclosing;
  private readonly Dictionary<string, GSObject> values = new Dictionary<string, GSObject>();

  public GSharpEnvironment(IEnvironment enclosing = null)
  {
    this.enclosing = (GSharpEnvironment)enclosing;
  }

  public void Define(Token name, GSObject value)
  {
    if (name.lexeme == "_") return;

    if (values.ContainsKey(name.lexeme))
    {
      throw new RuntimeError(name, "Variable with this name already declared in this scope.", null);
    }
    else
    {
      values[name.lexeme] = value;
    }
  }

  public GSObject GetAt(int distance, string name)
  {
    if (Ancestor(distance).values.ContainsKey(name))
    {
      return Ancestor(distance).values[name];
    }

    return new Objects.Undefined();
  }

  public void AssignAt(int distance, Token name, GSObject value)
  {
    if (name.lexeme == "_") return;

    Ancestor(distance).values[name.lexeme] = value;
  }

  private GSharpEnvironment Ancestor(int distance)
  {
    GSharpEnvironment environment = this;
    for (int i = 0; i < distance; i++)
    {
      environment = environment.enclosing;
    }

    return environment;
  }

  internal GSObject Get(Token name)
  {
    return Get(name.lexeme);
  }

  internal GSObject Get(string name)
  {
    if (values.ContainsKey(name))
    {
      return values[name];
    }

    return enclosing?.Get(name);
  }

  internal void Assign(Token name, GSObject value)
  {
    if (name.lexeme == "_") return;

    if (values.ContainsKey(name.lexeme))
    {
      values[name.lexeme] = value;
      return;
    }

    if (enclosing != null)
    {
      enclosing.Assign(name, value);
      return;
    }

    throw new RuntimeError(name, "Undefined variable '" + name.lexeme + "'.", null);
  }
}