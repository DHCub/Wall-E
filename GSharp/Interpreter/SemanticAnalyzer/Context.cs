namespace GSharp.Interpreter;
using System.Collections.Generic;
using GSharp.Statement;
using GSharp.Types;

public class VariableContext
{
    private Dictionary<string, VariableSymbol> variables;

    private readonly VariableContext enclosing;

    public VariableContext(VariableContext enclosing = null)
    {
        this.enclosing = enclosing;
        this.variables = new();
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

    public enum ReassignErorCode
    {
        NoError,
        UndefinedVariable,
        NonMatchingtypes
    }

    /// <summary>
    /// Reassigns a variable of a given name to a given symbol in the current context or closest enclosing wich has it defined
    /// </summary>
    /// <param name="name">Name of variable to reassign</param>
    /// <param name="symbol">New Symbol to Assign</param>
    /// <returns>ReassignErrorCode, along with the type placed in the Symbol after the call, which will be the most restricted between the type in the passed symbol and that of the old symbol and null if it was not defined</returns>
    public (ReassignErorCode errorCode, GSType? type) Reassign(string name, VariableSymbol symbol)
    {
        (ReassignErorCode errorCode, GSType? type) aux(VariableContext context)
        {
            if (context.variables.ContainsKey(name))
            {
                var oldSymbol = context.variables[name];
                if (!symbol.Type.SameTypeAs(oldSymbol.Type))
                {
                    return (ReassignErorCode.NonMatchingtypes, oldSymbol.Type);
                }
                
                var type = context.variables[name].Type;
                context.variables[name] = new(symbol.Type.GetMostRestrictedOrError(type), name);
                return (ReassignErorCode.NoError, context.variables[name].Type);
            }
            if (context.enclosing == null) return (ReassignErorCode.UndefinedVariable, null);
            return aux(context.enclosing);
        }

        return aux(this);
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