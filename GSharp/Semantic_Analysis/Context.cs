namespace GSharp;
using System.Collections.Generic;

public class Context
{
    
    private Dictionary<(string name, int? parameter_Number), Symbol> Assignments;

    #nullable enable

    private readonly Context? Enclosing;

    #nullable disable

    public Context(Context Enclosing = null)
    {
        this.Enclosing = Enclosing;
    }

    #nullable enable

    public Fun_Symbol? Get_Symbol(string name, int parameter_Number)
    {
        if (Assignments.ContainsKey((name, parameter_Number))) return (Fun_Symbol)Assignments[(name, parameter_Number)];
        else if(Enclosing != null) return Enclosing.Get_Symbol(name, parameter_Number);
        
        return null;
    }

    public Variable_Symbol? Get_Symbol(string name)
    {
        if (Assignments.ContainsKey((name, null))) return (Variable_Symbol)Assignments[(name, null)];
        else if(Enclosing != null) return Enclosing.Get_Symbol(name);
        
        return null;
    }


    public bool Define(string name, Variable_Symbol symbol)
    {
        if (this.Get_Symbol(name) != null) return false;

        Assignments[(name, null)] = symbol;
        return true;
    }

    public bool Define(string name, Fun_Symbol symbol, int parameter_Number)
    {
        if (this.Get_Symbol(name, parameter_Number) != null) return false;

        Assignments[(name, parameter_Number)] = symbol;
        return true;
    }

}