using GSharp.Exceptions;
using GSharp.Objects;
using GSharp.Objects.Collections;
using GSharp.Objects.Figures;
using GSharp.Statement;
using GSharp;
using System;
using System.Collections.Generic;
using GSharp.Types;

namespace GSharp.Interpreter;

internal class GSFunction : GSObject, ICallable, IFunction
{
  private readonly Function declaration;
  private readonly IEnvironment closure;
  public Stack<string> importStack;

  internal GSFunction(Function declaration, IEnvironment closure, Stack<string> importStack)
  {
    this.declaration = declaration;
    this.closure = closure;
    this.importStack = importStack;
  }

  public GSObject Call(IInterpreter interpreter, List<GSObject> arguments)
  {
    var environment = new GSharpEnvironment(closure);

    for (int i = 0; i < declaration.Parameters.Count; i++)
    {
      if (declaration.Parameters[i].typeName is not null && !arguments[i].SameTypeAs(declaration.Parameters[i].typeName))
      {
        throw new RuntimeError(declaration.Token, $"Function takes `{declaration.Parameters[i].typeName}` as its #{i + 1} parameter, `{arguments[i].GetType()}` passed instead", importStack);
      }

      environment.Define(declaration.Parameters[i].Name, arguments[i]);
    }

    try
    {
      interpreter.ExecuteBlock(declaration.Body, environment);
      return new Objects.Undefined();
    }
    catch (Exceptions.Return returnValue)
    {
      if (declaration.ReturnTypeName is not null && returnValue.Value.SameTypeAs(declaration.ReturnTypeName))
      {
        throw new RuntimeError(declaration.Token, $"Function returns `{declaration.ReturnTypeName}`, `{returnValue.Value}` passed instead", importStack);
      }

      return returnValue.Value;
    }
  }

  public override bool SameTypeAs(GSObject gso) => throw new NotImplementedException();

  public int Arity()
  {
    return declaration.Parameters.Count;
  }

  public override string ToString()
  {
    return "<fun " + declaration.Name.lexeme + ">";
  }

  public override bool Equals(GSObject obj)
  {
    throw new NotImplementedException();
  }

  public override string GetTypeName()
  {
    throw new NotImplementedException();
  }

  public override bool GetTruthValue()
  {
    throw new NotImplementedException();
  }

  public override GSObject OperatePoint(Point other, Add op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperatePoint(Point other, Subst op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperatePoint(Point other, Mult op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateScalar(Scalar other, Add op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateScalar(Scalar other, Subst op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateScalar(Scalar other, Mult op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateScalar(Scalar other, Div op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateScalar(Scalar other, Mod op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateScalar(Scalar other, LessTh op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateScalar(Scalar other, Indexer op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateMeasure(Measure other, Add op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateMeasure(Measure other, Subst op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateMeasure(Measure other, Mult op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateMeasure(Measure other, Div op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateMeasure(Measure other, Mod op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateMeasure(Measure other, LessTh op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateString(Objects.String other, Add op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateUndefined(Undefined other, Add op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op)
  {
    throw new NotImplementedException();
  }

  public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op)
  {
    throw new NotImplementedException();
  }

  public override bool SameTypeAs(GSType gst)
  {
    throw new NotImplementedException();
  }
}