using System;
using System.Collections.Generic;

namespace GSharp.Exceptions;

public class SemanticError : Exception
{
    public readonly Token token;
    private readonly Stack<string> ImportTrace;
    public SemanticError(Token token, string message, Stack<string> ImportTrace) : base(message)
    {
        this.token = token;
        this.ImportTrace = ImportTrace;
    }
    public override string ToString() 
    {
        var answ = new List<char>();
        answ.AddRange($"! SEMANTIC ERROR: at {token.line}:{token.column} '{token.lexeme}': {base.Message}");
    
        answ.AddRange(ImportTraceBuilder.GetImportTrace(ImportTrace));


        return new string(answ.ToArray());
    }
}