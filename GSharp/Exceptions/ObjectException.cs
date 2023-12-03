namespace GSharp.Exceptions;

public abstract class ObjectException : RuntimeError
{
  protected ObjectException(string message) : base(null, message) { }
}