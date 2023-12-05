namespace GSharp.Interpreter;
using System.Collections.Generic;
using GSharp.Types;

public abstract class Symbol
{

}

public class FunSymbol : Symbol
{
    public readonly List<(GSType Type, string Name)> Parameters;
    public readonly GSType ReturnType;
    public readonly string Name;


    public FunSymbol(string Name, List<(GSType, string)> Parameters, GSType ReturnType)
    {
        this.Name = Name;
        this.Parameters = Parameters;
        this.ReturnType = ReturnType;
    }
}

public class VariableSymbol : Symbol
{
    public readonly string Name;
    public readonly GSType Type;

    public VariableSymbol(GSType Type,  string Name)
    {
        this.Type = Type;
        this.Name = Name;
    }
}