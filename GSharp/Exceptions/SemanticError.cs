using System;

namespace GSharp.Exceptions;

public class SemanticError : Exception
{
    private readonly Token token;
    public SemanticError(Token token, string message) : base(message)
    {
        this.token = token;
    }
    public override string ToString() 
        => $"! SEMANTIC ERROR: at {token.line}:{token.column} '{token.lexeme}': {base.Message}";
}