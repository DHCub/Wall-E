namespace GSharp.Exceptions;

public class IllegalStateException : CollectionException
{
  public IllegalStateException(string message) : base(message) { }
}