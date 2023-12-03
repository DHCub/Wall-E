namespace GSharp.Statement;

using GSharp.Expression;

public class Var : Stmt, IToken
{
  public readonly Token Name;
  public readonly Expr Initializer;
  public readonly ITypeReference TypeReference;

  public Var(Token Name, Expr Initializer, TypeReference TypeReference)
  {
    this.Name = Name;
    this.Initializer = Initializer;
    this.TypeReference = TypeReference;
  }

  public override R Accept<R>(IVisitor<R> visitor)
  {
    return visitor.VisitVarStmt(this);
  }

  public override string ToString()
  {
    if (Initializer != null)
    {
      if (TypeReference?.TypeSpecifier.lexeme is not null)
      {
        return $"var {Name.lexeme}: {TypeReference.TypeSpecifier.lexeme} = {Initializer};";
      }
      else
      {
        return $"var {Name.lexeme} = {Initializer};";
      }
    }
    else
    {
      if (TypeReference?.TypeSpecifier.lexeme is not null)
      {
        return $"var {Name.lexeme}: {TypeReference.TypeSpecifier.lexeme};";
      }
      else
      {
        return $"var {Name.lexeme};";
      }
    }
  }

  public Token Token => Name;
}