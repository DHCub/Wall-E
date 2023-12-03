namespace GSharp.Exceptions;

public class IOExceptions : ObjectException
{
  public int errorNumber { get; }

  public IOExceptions(string message, int errorNumber) : base($"Error {errorNumber}: {message}")
  {
    this.errorNumber = errorNumber;
  }
}