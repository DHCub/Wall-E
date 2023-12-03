namespace GSharp;
using System.Collections.Generic;
using GSharp.Types;

public abstract class Symbol
{

}

public class Fun_Symbol : Symbol
{
    public readonly List<(GSType Type, string Name)> Parameters;
    public readonly GSType ReturnType;
    public readonly string Name;


    public Fun_Symbol(string Name, List<(GSType, string)> Parameters, GSType ReturnType)
    {
        this.Name = Name;
        this.Parameters = Parameters;
        this.ReturnType = ReturnType;
    }
}

public class Variable_Symbol : Symbol
{
    public readonly string Name;
    public readonly GSType Type;

    public Variable_Symbol(GSType Type,  string Name)
    {
        this.Type = Type;
        this.Name = Name;
    }
}