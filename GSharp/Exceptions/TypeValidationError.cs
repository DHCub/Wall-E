namespace GSharp.Exceptions;

public class TypeValidationError : ValidationError
{
  public TypeValidationError(Token token, string message) : base(token, message)
  {

  }
}