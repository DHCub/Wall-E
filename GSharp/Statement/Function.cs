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

  public Function(Token Name, IEnumerable<Parameter> Parameters, IEnumerable<Stmt> Body, TypeReference ReturnTypeReference, TypeName? ReturnTypeName)
  {
    this.Name = Name;
    this.Parameters = Parameters.ToImmutableList();
    this.Body = Body.ToImmutableList();
    this.ReturnTypeReference = ReturnTypeReference;
    this.ReturnTypeName = ReturnTypeName;
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