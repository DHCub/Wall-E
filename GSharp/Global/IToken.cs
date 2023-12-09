namespace GSharp;

/// <summary>
/// interface for expressions and statements which provide a Token.
/// this is used to augment error handling, for a better end-user experience
/// </summary>
public interface IToken
{
  // gets the token that this expression represents, or a token close to it.
  // if 'null' the token is unknown
  public Token? Token { get; }
}