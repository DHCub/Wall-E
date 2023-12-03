namespace GSharp.Exceptions;

public class IllegalStateException : ObjectException
{
  public IllegalStateException(string message) : base(message) { }
}