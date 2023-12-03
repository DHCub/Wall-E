namespace GSharp.Exceptions;

public class NameResolutionTypeValidationError : TypeValidationError
{
  public NameResolutionTypeValidationError(Token token, string message) : base(token, message)
  {

  }
}