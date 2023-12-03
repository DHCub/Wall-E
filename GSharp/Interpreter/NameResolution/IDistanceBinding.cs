namespace GSharp.Interpreter;

internal interface IDistanceBinding
{
  // gets the distance (number of scopes) from the referring expressiong to the
  // scope in which the name is declared.
  // 0 = same scope, 1 = one scope above, ...
  // 
  // the magic value -1 is used to indicate that the binding refers to a global function.
  int Distance { get; }
}