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
    
        if (ImportTrace == null || ImportTrace.Count == 0) return new string(answ.ToArray());

        answ.AddRange("\nAt File:\n");
        bool first = true;
        foreach(var file in ImportTrace)
        {
            if (!first)
            {
                answ.AddRange(", Imported from File:\n");
                answ.AddRange(file);
            }
            else
            {
                answ.AddRange(file);
                first = false;
            }
        }

        return new string(answ.ToArray());
    }
}