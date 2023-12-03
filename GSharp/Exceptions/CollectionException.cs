namespace GSharp.Exceptions;

public abstract class CollectionException : RuntimeError
{
  protected CollectionException(string message) : base(null, message) { }
}