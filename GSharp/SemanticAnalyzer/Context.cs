namespace GSharp.SemanticAnalyzer;
using System.Collections.Generic;
using GSharp.Statement;

public class VariableContext
{
    private Dictionary<string, VariableSymbol> variables;

    private readonly VariableContext enclosing;

    public VariableContext(VariableContext enclosing = null)
    {
        this.enclosing = enclosing ?? new();
    }

    public VariableSymbol? GetSymbol(string name)
    {
        if (variables.ContainsKey(name)) return variables[name];
        else if(enclosing != null) return enclosing.GetSymbol(name);
        
        return null;
    }

    public bool Define(string name, VariableSymbol symbol)
    {
        if (this.GetSymbol(name) != null) return false;

        variables[name] = symbol;
        return true;
    }

}

public class FunctionContext
{   
    private Dictionary<(string name, int parameter_Number), FunSymbol> functions;

    private readonly FunctionContext? enclosing;

    public FunctionContext(FunctionContext enclosing = null)
    {
        this.enclosing = enclosing;
        this.functions = new();
    }

    public FunSymbol? GetSymbol(string name, int parameterNumber)
    {
        if (functions.ContainsKey((name, parameterNumber))) return functions[(name, parameterNumber)];
        else if(enclosing != null) return enclosing.GetSymbol(name, parameterNumber);
        
        return null;
    }


    public bool Define(string name, FunSymbol symbol)
    {
        var parameterNumber = symbol.Parameters.Count;
        
        if (this.GetSymbol(name, parameterNumber) != null) return false;

        functions[(name, parameterNumber)] = symbol;
        return true;
    }

}