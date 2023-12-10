namespace GSharp.Statement;

using GSharp.Expression;
using GSharp.Types;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

public class Function : Stmt, IToken
{
  public readonly Token Name;
  public readonly ImmutableList<Parameter> Parameters;
  public readonly ImmutableList<Stmt> Body;
  public readonly ITypeReference ReturnTypeReference;
  public readonly TypeName? ReturnTypeName;

  public Function(Token name, IEnumerable<Parameter> parameters, IEnumerable<Stmt> body, TypeReference returnTypeReference, TypeName? returnTypeName)
  {
    this.Name = name;
    this.Parameters = parameters.ToImmutableList();
    this.Body = body.ToImmutableList();
    this.ReturnTypeReference = returnTypeReference;
    this.ReturnTypeName = returnTypeName;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitFunctionStmt(this);
  }

  public override string ToString()
  {
    return $"fun {Name.lexeme}(): {ReturnTypeReference.CSharpType}";
  }

  public Token Token => Name;
}