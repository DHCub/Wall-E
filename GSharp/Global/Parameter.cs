namespace GSharp;

public class Parameter
{
  public Token Name { get; }
  public ITypeReference TypeReference { get; }

  public Token TypeSpecifier => TypeReference.TypeSpecifier;

  public Parameter(ITypeReference TypeReference)
  {
    this.TypeReference = TypeReference;
  }

  public Parameter(Token Name, ITypeReference TypeReference)
  {
    this.Name = Name;
    this.TypeReference = TypeReference;
  }
}