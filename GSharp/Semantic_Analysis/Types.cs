using System.Diagnostics.Metrics;

namespace GSharp;

public abstract class GSharpType 
{
    public enum Types
    {
        Point,
        Line,
        Ray,
        Segment,
        Circle,
        Arc,
        Scalar,
        Measure,
        Boolean,
        String,
    }

    public abstract bool Matches(GSharpType other);
    public abstract override string ToString();
    public abstract bool IsUndefined();

    public abstract bool IsAddable();
    public abstract bool IsSubstractable();
    public abstract bool IsComparable();
}

public class Constant_SimpleType : GSharpType
{
    public readonly Types Type;

    public Constant_SimpleType(Types Type)
    {
        this.Type = Type;
    }

    public override bool Matches(GSharpType other)
    {
        if (other is Constant_SimpleType CST)
            return this.Type == CST.Type;
        
        return other.Matches(this);
    }

    public override string ToString() => this.Type.ToString();

    public override bool IsUndefined() => false;

    public override bool IsAddable()
    {
        return this.Type switch
        {
            Types.Scalar or Types.Measure or Types.String => true,
            _ => false,
        };
    }

    public override bool IsSubstractable()
    {
        return this.Type switch
        {
            Types.Measure or Types.Scalar => true,
            _ => false,
        };
    }

    public override bool IsComparable()
    {
        return this.Type switch
        {
            Types.Scalar or Types.Measure => true,
            _ => false
        };
    }
}

public class Sequence_Type : GSharpType
{
    public readonly GSharpType Type;

    public Sequence_Type(GSharpType Type)
    {
        this.Type = Type;
    }

    public override bool Matches(GSharpType other)
        => other is Sequence_Type ST && this.Type.Matches(ST.Type) ||
           other is Undefined_Type || 
           other is Drawable_Type d && this.Type.Matches(d) ||
           other is Constant_SimpleType CST && CST.Type == Types.Boolean;

    public override string ToString() => $"Seq<{this.Type.ToString()}>";

    public override bool IsUndefined() => this.Type.IsUndefined();

    public override bool IsAddable() => true;

    public override bool IsComparable() => false;

    public override bool IsSubstractable() => false;
}

public class Drawable_Type : GSharpType
{
    public Drawable_Type() {}

    public override bool Matches(GSharpType other)
    {
        if (other is Drawable_Type) return true;
        if (other is Sequence_Type seq) return this.Matches(seq.Type);
        if (other is Constant_SimpleType cT)
        {
            return cT.Type switch
            {
                Types.Point or Types.Line or Types.Ray or Types.Segment or Types.Circle or Types.Arc => true,
                _ => false,
            };
        }
        if (other is Undefined_Type) return true;

        return false;
    }

    public override string ToString() => "FIGURE";

    public override bool IsUndefined() => false;
    public override bool IsAddable() => false;

    public override bool IsSubstractable() => false;

    public override bool IsComparable() => false;
}

public class Undefined_Type : GSharpType
{
    public Undefined_Type() {}

    public override bool Matches(GSharpType other) => true;

    public override string ToString() => "UNDEFINED_TYPE";

    public override bool IsUndefined() => true;
    public override bool IsAddable()=> true;

    public override bool IsComparable() => true;

    public override bool IsSubstractable() => true;
}

// public class Type_Variable : GSharpType
// {
//     private static ulong Counter = 0;

//     private readonly ulong ID;

//     public Type_Variable()
//     {
//         this.ID = Counter++;
//     }

//     public override bool Equals(object obj)
//     {
//         return obj is Type_Variable t && t.ID == this.ID;
//     }

//     public override bool Equals(GSharpType other)
//         => other is Type_Variable TV && TV == this;

//     public static void Reset_Names() {Counter = 0;}

//     // public override int GetHashCode()
//     // {
//     //     return base.GetHashCode();
//     // }
// }

// public class Template_Type : GSharpType
// {
//     private static ulong Counter;

//     private readonly ulong ID;

//     public Template_Type()
//     {
//         ID = Counter++;
//     }

//     public override bool Equals(GSharpType other)
//         => other is Template_Type TT && TT.ID == this.ID;
// }

