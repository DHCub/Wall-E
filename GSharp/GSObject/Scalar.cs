namespace GSharp.Objects;

using System;
using GSharp.Objects.Collections;
using GSharp.Objects.Figures;
using GSharp.Types;

public class Scalar : GSObject
{
    public readonly double value;

    public Scalar(double value)
    {
        this.value = value;
    }

    public Scalar(bool value)
    {
        this.value = (value)? 1 : 0;
    }

    public static implicit operator Scalar(double d) => new(d);
    public static implicit operator double(Scalar s) => s.value;

    public static implicit operator Scalar(int i) => new(i);
    public static explicit operator int(Scalar s) => (int)s.value;

    public override bool GetTruthValue() => !Functions.Equal_Approx(value, 0);
    public override string GetTypeName() => TypeName.Scalar.ToString();

    public override GSObject OperatePoint(Point P, Add op) => UnsupportedOperError(P, op);

    public override GSObject OperatePoint(Point other, Mult op)
        => this.value*other;

    public override GSObject OperatePoint(Point other, Subst op) => UnsupportedOperError(other, op);

    public override GSObject OperateScalar(Scalar other, Add op) => new Scalar(this.value + other.value);
    public override GSObject OperateScalar(Scalar other, Subst op) => new Scalar(this.value - other.value);
    public override GSObject OperateScalar(Scalar other, Mult op) => new Scalar(this.value * other.value);
    public override GSObject OperateScalar(Scalar other, Div op) => new Scalar(this.value / other.value);

    public override GSObject OperateScalar(Scalar other, Mod op) => new Scalar(this.value % other.value);
    // {
    //     int ConvertToIntegerOrError(double x)
    //     {
    //         if (Functions.Equal_Approx(x - Math.Floor(x), 0)) return (int)Math.Floor(x);
    //         if (Functions.Equal_Approx(x - Math.Ceiling(x), 0)) return (int)Math.Ceiling(x);
    //             throw new RuntimeError("Tried to find modulo of non-integer values");
    //     }

    //     return new Scalar(ConvertToIntegerOrError(this.value)%ConvertToIntegerOrError(other.value));   
    // }

    public override GSObject OperateScalar(Scalar other, LessTh op)
        => new Scalar(Functions.Less_Than_Approx(this.value, other.value)? 1 : 0);

    public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op)
        => UnsupportedOperError(other, op);

    public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op)
        => UnsupportedOperError(other, op);

    public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op)
        => UnsupportedOperError(other, op);

    public override GSObject OperateString(String other, Add op)
        => UnsupportedOperError(other, op);

    public override GSObject OperateUndefined(Undefined other, Add op) => UnsupportedOperError(other, op);

    public override GSObject OperateMeasure(Measure other, Add op) => UnsupportedOperError(other, op);
    public override GSObject OperateMeasure(Measure other, Subst op) => UnsupportedOperError(other, op);
    public override GSObject OperateMeasure(Measure other, Mult op) => other.OperateScalar(this, op);
    public override GSObject OperateMeasure(Measure other, Div op) => UnsupportedOperError(other, op);
    public override GSObject OperateMeasure(Measure other, Mod op) => UnsupportedOperError(other, op);
    public override GSObject OperateMeasure(Measure other, LessTh op) => new Scalar(Functions.Less_Than_Approx(Math.Abs(this.value), other.value));


    public override string ToString() => (this.value).ToString();

    public override bool Equals(GSObject obj) => obj is Scalar s && Functions.Equal_Approx(s.value, this.value);


}