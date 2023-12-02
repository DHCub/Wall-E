namespace GSharp.GSObject;

using System;
using GSharp.GSObject.Collections;
using GSharp.GSObject.Figures;

public class Scalar : GSObject
{
    public readonly double value;

    public Scalar(double value)
    {
        this.value = value;
    }

    public override bool GetTruthValue() => !Functions.Equal_Approx(value, 0);
    public override string GetTypeName() => GSTypes.Scalar.ToString();

    public override GSObject OperatePoint(Point P, Add op) => UnsupportedOperation(P, op);

    public override GSObject OperatePoint(Point other, Mult op)
        => this.value*other;

    public override GSObject OperatePoint(Point other, Subst op) => UnsupportedOperation(other, op);

    public override GSObject OperateScalar(Scalar other, Add op)
        => new Scalar(this.value + other.value);

    public override GSObject OperateScalar(Scalar other, Subst op)
        => new Scalar(this.value - other.value);

    public override GSObject OperateScalar(Scalar other, Mult op)
        => new Scalar(this.value * other.value);

    public override GSObject OperateScalar(Scalar other, Div op)
        => new Scalar(this.value / other.value);

    public override GSObject OperateScalar(Scalar other, Mod op)
    {
        int ConvertToIntegerOrError(double x)
        {
            if (Functions.Equal_Approx(x - Math.Floor(x), 0)) return (int)Math.Floor(x);
            if (Functions.Equal_Approx(x - Math.Ceiling(x), 0)) return (int)Math.Ceiling(x);
                throw new RuntimeError("Tried to find modulo of non-integer values");
        }

        return new Scalar(ConvertToIntegerOrError(this.value)%ConvertToIntegerOrError(other.value));   
    }

    public override GSObject OperateScalar(Scalar other, LessTh op)
        => new Boolean(Functions.Less_Than_Approx(this.value, other.value));

    public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op)
        => UnsupportedOperation(other, op);

    public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op)
        => UnsupportedOperation(other, op);

    public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op)
        => UnsupportedOperation(other, op);

    public override GSObject OperateString(String other, Add op)
        => UnsupportedOperation(other, op);

    public override GSObject OperateUndefined(Undefined other, Add op) => UnsupportedOperation(other, op);

    public override string ToString() => (this.value).ToString();

    public override bool Equals(GSObject obj) => obj is Scalar s && Functions.Equal_Approx(s.value, this.value);


}