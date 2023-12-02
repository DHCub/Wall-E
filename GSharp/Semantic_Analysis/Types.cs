using System.Diagnostics.Metrics;

namespace GSharp;

public enum TypeName
{
    Point,
    Line,
    Ray,
    Segment,
    Circle,
    Arc,
    Scalar,
    Measure,
    String,
}

public abstract class GSharpType 
{

    public abstract bool Matches(GSharpType other);
    public abstract override string ToString();
    public abstract bool IsUndefined();

    public abstract bool IsAddable();
    public abstract bool IsSubstractable();
    public abstract bool IsComparable();

    public abstract bool Is_Multiplyable_By(GSharpType other);

    public abstract bool Is_Dividable_By(GSharpType other);
}

public class Constant_SimpleType : GSharpType
{
    public readonly TypeName Type;

    public Constant_SimpleType(TypeName Type)
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
            TypeName.Point or TypeName.Scalar or TypeName.Measure or TypeName.String => true,
            _ => false,
        };
    }

    public override bool IsSubstractable()
    {
        return this.Type switch
        {
            TypeName.Point or TypeName.Measure or TypeName.Scalar => true,
            _ => false,
        };
    }

    public override bool IsComparable()
    {
        return this.Type switch
        {
            TypeName.Scalar or TypeName.Measure => true,
            _ => false
        };
    }

    public override bool Is_Multiplyable_By(GSharpType other)
    {
        if (other is Constant_SimpleType CST)
        {
            if (this.Type == TypeName.Scalar)
            {
                return CST.Type switch{
                    TypeName.Point or TypeName.Measure or TypeName.Scalar => true,
                    _ => false
                };
            }

            if (this.Type == TypeName.Measure)
            {
                return CST.Type == TypeName.Scalar;
            }

            if (this.Type == TypeName.Point)
            {
                return CST.Type == TypeName.Scalar;
            }

            return false;
        }

        return other.Is_Multiplyable_By(this); // multiplication is conmutative in this case
    }

    public override bool Is_Dividable_By(GSharpType other)
    {
        if (other is Constant_SimpleType CST)
        {
            if (CST.Type != TypeName.Scalar) return false;
            
            return this.Type switch{
                TypeName.Scalar or TypeName.Point or TypeName.Measure => true,
                _ => false
            };
        }

        if (other is Drawable_Type) return false;

        if (other is Sequence_Type) return false;

        if (other is Undefined_Type) return false;

        throw new System.Exception("UNRECOGNIZED TYPE");
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
           other is Constant_SimpleType CST && CST.Type == TypeName.Boolean;

    public override string ToString() => $"Seq<{this.Type.ToString()}>";

    public override bool IsUndefined() => this.Type.IsUndefined();

    public override bool IsAddable() => true;

    public override bool IsComparable() => false;

    public override bool IsSubstractable() => false;

    public override bool Is_Multiplyable_By(GSharpType other) => false;

    public override bool Is_Dividable_By(GSharpType other) => false;
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
                TypeName.Point or TypeName.Line or TypeName.Ray or TypeName.Segment or TypeName.Circle or TypeName.Arc => true,
                _ => false,
            };
        }
        if (other is Undefined_Type) return true;

        return false;
    }

    public override string ToString() => "FIGURE";

    public override bool IsUndefined() => false;
    public override bool IsAddable() => true; // bc points are addable

    public override bool IsSubstractable() => true; // bc points are substractable

    public override bool IsComparable() => false;

    public override bool Is_Multiplyable_By(GSharpType other) 
        => other is Constant_SimpleType CST && CST.Type == TypeName.Scalar;

    public override bool Is_Dividable_By(GSharpType other)
    {
        if (other is not Constant_SimpleType CST) return false;

        return CST.Type == TypeName.Scalar;
    }
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

    public override bool Is_Multiplyable_By(GSharpType other) => true;

    public override bool Is_Dividable_By(GSharpType other) => true;
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

