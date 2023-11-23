namespace GSharp;
using System.Collections.Generic;

public abstract class Symbol
{

}

public class Fun_Symbol : Symbol
{
    public readonly List<(GSharpType Type, string Name)> Parameters;
    public readonly GSharpType ReturnType;
    public readonly string Name;

// #nullable enable
//     public readonly Equation_System? Parameter_Constraints;
// #nullable disable

    public Fun_Symbol(string Name, List<(GSharpType, string)> Parameters, GSharpType ReturnType)//, Equation_System Parameter_Constraints = null)
    {
        this.Name = Name;
        this.Parameters = Parameters;
        this.ReturnType = ReturnType;
        // this.Parameter_Constraints = Parameter_Constraints;
    }
}

public class Variable_Symbol : Symbol
{
    public readonly string Name;
    public readonly GSharpType Type;

    public Variable_Symbol(GSharpType Type,  string Name)
    {
        this.Type = Type;
        this.Name = Name;
    }
}