namespace GSharp.Exceptions;

// <summary>
// Base class for different kinds of validation errors.
// </summary>
public abstract class ValidationError : InterpreterException
{
  // gets the approximate location at which the error ocurred
  public Token Token { get; }

  protected ValidationError(Token token, string message) : base(message)
  {
    this.Token = token;
  }
}