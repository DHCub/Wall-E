namespace GSharp.Objects;

using System;
using GSharp.Objects.Collections;
using GSharp.Objects.Figures;
using GSharp.Types;
using GSharp.Exceptions;

public abstract class GSObject : IOperate<Add>,
                                IOperate<Subst>,
                                IOperate<Mult>,
                                IOperate<Div>,
                                IOperate<Mod>,
                                IOperate<Power>,
                                IOperate<LessTh>
{
    public abstract override string ToString();
    public abstract bool Equals(GSObject obj);
    public abstract string GetTypeName();
    public abstract bool GetTruthValue();
    public abstract bool SameTypeAs(GSObject gso);

    protected static string NoOrderRelation(string T1, string T2)
        => $"No order relation between {T1} and {T2}";

    protected static string NoOrderRelation(string T1, TypeName T2)
        => $"No order relation between {T1} and {T2}";

    protected static string TriedToOperate(string operation, string T1, string T2)
        => $"Tried to {operation} {T1} with {T2}";

    protected static string TriedToOperate(string operation, string T1, TypeName T2)
        => $"Tried to {operation} {T1} with {T2}";

    protected static string TriedToFindModuloOfT1OverT2(string T1, string T2)
        => $"Tried to find Modulo of {T1} over {T2}";

    protected static string TriedToFindModuloOfT1OverT2(string T1, TypeName T2)
        => $"Tried to find Modulo of {T1} over {T2}";

    protected static string TriedToElevate(string T1, string T2)
        => $"Tried to elevate {T1} to the power of {T2}";

    #region Type Names

    // protected const string POINT = "Point";
    // protected const string LINE = "Line";
    // protected const string SEGMENT = "Segment";
    // protected const string RAY = "Ray";
    // protected const string CIRCLE = "Circle";
    // protected const string ARC = "Arc";

    // protected const string BOOLEAN = "Boolean";
    // protected const string STRING = "String";
    // protected const string SCALAR = "Scalar";

    protected const string SEQUENCE = "Sequence";
    protected const string UNDEFINED = "Undefined";

    #endregion


    #region Errors

    public GSObject UnsupportedOperError(GSObject other, Add OP) => throw new RuntimeError(null, TriedToOperate("Add", this.GetTypeName(), other.GetTypeName()));
    public GSObject UnsupportedOperError(GSObject other, Subst OP) => throw new RuntimeError(null, TriedToOperate("Substract", this.GetTypeName(), other.GetTypeName()));
    public GSObject UnsupportedOperError(GSObject other, Mult OP) => throw new RuntimeError(null, TriedToOperate("Multiply", this.GetTypeName(), other.GetTypeName()));
    public GSObject UnsupportedOperError(GSObject other, Div OP) => throw new RuntimeError(null, TriedToOperate("Divide", this.GetTypeName(), other.GetTypeName()));
    public GSObject UnsupportedOperError(GSObject other, Mod OP) => throw new RuntimeError(null, TriedToFindModuloOfT1OverT2(this.GetTypeName(), other.GetTypeName()));
    public GSObject UnsupportedOperError(GSObject other, Power OP) => throw new RuntimeError(null, TriedToElevate(this.GetTypeName(), other.GetTypeName()));
    public GSObject UnsupportedOperError(GSObject other, LessTh OP) => throw new RuntimeError(null, NoOrderRelation(this.GetTypeName(), other.GetTypeName()));

    #endregion


    #region OperatePoint

    public abstract GSObject OperatePoint(Point other, Add op);
    public abstract GSObject OperatePoint(Point other, Subst op);
    public abstract GSObject OperatePoint(Point other, Mult op);
    public GSObject OperatePoint(Point other, Div op) => UnsupportedOperError(other, op);
    public GSObject OperatePoint(Point other, Mod op) => UnsupportedOperError(other, op);
    public GSObject OperatePoint(Point other, Power op) => UnsupportedOperError(other, op);
    public GSObject OperatePoint(Point other, LessTh op) => UnsupportedOperError(other, op);

    #endregion

    #region OperateLine

    public GSObject OperateLine(Line other, Add op) => UnsupportedOperError(other, op);
    public GSObject OperateLine(Line other, Subst op) => UnsupportedOperError(other, op);
    public GSObject OperateLine(Line other, Mult op) => UnsupportedOperError(other, op);
    public GSObject OperateLine(Line other, Div op) => UnsupportedOperError(other, op);
    public GSObject OperateLine(Line other, Mod op) => UnsupportedOperError(other, op);
    public GSObject OperateLine(Line other, Power op) => UnsupportedOperError(other, op);
    public GSObject OperateLine(Line other, LessTh op) => UnsupportedOperError(other, op);

    #endregion

    #region OperateRay
    public GSObject OperateRay(Ray other, Add op) => UnsupportedOperError(other, op);
    public GSObject OperateRay(Ray other, Subst op) => UnsupportedOperError(other, op);
    public GSObject OperateRay(Ray other, Mult op) => UnsupportedOperError(other, op);
    public GSObject OperateRay(Ray other, Div op) => UnsupportedOperError(other, op);
    public GSObject OperateRay(Ray other, Mod op) => UnsupportedOperError(other, op);
    public GSObject OperateRay(Ray other, Power op) => UnsupportedOperError(other, op);
    public GSObject OperateRay(Ray other, LessTh op) => UnsupportedOperError(other, op);

    #endregion

    #region OperateSegment
    public GSObject OperateSegment(Segment other, Add op) => UnsupportedOperError(other, op);
    public GSObject OperateSegment(Segment other, Subst op) => UnsupportedOperError(other, op);
    public GSObject OperateSegment(Segment other, Mult op) => UnsupportedOperError(other, op);
    public GSObject OperateSegment(Segment other, Div op) => UnsupportedOperError(other, op);
    public GSObject OperateSegment(Segment other, Mod op) => UnsupportedOperError(other, op);
    public GSObject OperateSegment(Segment other, Power op) => UnsupportedOperError(other, op);
    public GSObject OperateSegment(Segment other, LessTh op) => UnsupportedOperError(other, op);

    #endregion

    #region OperateCircle

    public GSObject OperateCircle(Circle other, Add op) => UnsupportedOperError(other, op);
    public GSObject OperateCircle(Circle other, Subst op) => UnsupportedOperError(other, op);
    public GSObject OperateCircle(Circle other, Mult op) => UnsupportedOperError(other, op);
    public GSObject OperateCircle(Circle other, Div op) => UnsupportedOperError(other, op);
    public GSObject OperateCircle(Circle other, Mod op) => UnsupportedOperError(other, op);
    public GSObject OperateCircle(Circle other, Power op) => UnsupportedOperError(other, op);
    public GSObject OperateCircle(Circle other, LessTh op) => UnsupportedOperError(other, op);

    #endregion

    #region OperateArc

    public GSObject OperateArc(Arc other, Add op) => UnsupportedOperError(other, op);
    public GSObject OperateArc(Arc other, Subst op) => UnsupportedOperError(other, op);
    public GSObject OperateArc(Arc other, Mult op) => UnsupportedOperError(other, op);
    public GSObject OperateArc(Arc other, Div op) => UnsupportedOperError(other, op);
    public GSObject OperateArc(Arc other, Mod op) => UnsupportedOperError(other, op);
    public GSObject OperateArc(Arc other, Power op) => UnsupportedOperError(other, op);
    public GSObject OperateArc(Arc other, LessTh op) => UnsupportedOperError(other, op);

    #endregion

    #region OperateScalar

    public abstract GSObject OperateScalar(Scalar other, Add op);
    public abstract GSObject OperateScalar(Scalar other, Subst op);
    public abstract GSObject OperateScalar(Scalar other, Mult op);
    public abstract GSObject OperateScalar(Scalar other, Div op);
    public abstract GSObject OperateScalar(Scalar other, Mod op);
    public GSObject OperateScalar(Scalar other, Power op)
    {
        (bool isInteger, int val) GetInteger(double d)
        {
            if (Functions.EqualApprox(d, double.Floor(d))) return (true, (int)double.Floor(d));
            if (Functions.EqualApprox(d, double.Ceiling(d))) return (true, (int)double.Ceiling(d));

            return (false, default);
        }

        if (this is Scalar thisScalar)
        {
            if (Functions.EqualApprox(other.value, 0)) return new Scalar(1);
            if (Functions.EqualApprox(thisScalar.value, 0))
            {
                if (Functions.GreaterThanApprox(other.value, 0)) return new Scalar(0);
                throw new RuntimeError(null, "Tried to elevate 0 to a negative exponent");
            }
            if (Functions.LessThanApprox(thisScalar.value, 0))
            {
                var (isInteger, val) = GetInteger(other.value);
                if (!isInteger) throw new RuntimeError(null, "Tried to elevate a negative number to a non-integer exponent");
                return new Scalar(Math.Pow(thisScalar.value, val));
            }

            return new Scalar(Math.Pow(thisScalar, other));
        }

        return UnsupportedOperError(other, op);
    }

    public abstract GSObject OperateScalar(Scalar other, LessTh op);

    #endregion

    #region OperateMeasure

    public abstract GSObject OperateMeasure(Measure other, Add op);
    public abstract GSObject OperateMeasure(Measure other, Subst op);
    public abstract GSObject OperateMeasure(Measure other, Mult op);
    public abstract GSObject OperateMeasure(Measure other, Div op);
    public abstract GSObject OperateMeasure(Measure other, Mod op);
    public GSObject OperateMeasure(Measure other, Power op) => UnsupportedOperError(other, op);
    public abstract GSObject OperateMeasure(Measure other, LessTh op);

    #endregion

    #region OperateString

    public abstract GSObject OperateString(String other, Add op);
    public GSObject OperateString(String other, Subst op) => UnsupportedOperError(other, op);
    public GSObject OperateString(String other, Mult op) => UnsupportedOperError(other, op);
    public GSObject OperateString(String other, Div op) => UnsupportedOperError(other, op);
    public GSObject OperateString(String other, Mod op) => UnsupportedOperError(other, op);
    public GSObject OperateString(String other, Power op) => UnsupportedOperError(other, op);
    public GSObject OperateString(String other, LessTh op) => UnsupportedOperError(other, op);

    #endregion

    #region OperateUndefined

    public abstract GSObject OperateUndefined(Undefined other, Add op);
    public GSObject OperateUndefined(Undefined other, Subst op) => UnsupportedOperError(other, op);
    public GSObject OperateUndefined(Undefined other, Mult op) => UnsupportedOperError(other, op);
    public GSObject OperateUndefined(Undefined other, Div op) => UnsupportedOperError(other, op);
    public GSObject OperateUndefined(Undefined other, Mod op) => UnsupportedOperError(other, op);
    public GSObject OperateUndefined(Undefined other, Power op) => UnsupportedOperError(other, op);
    public GSObject OperateUndefined(Undefined other, LessTh op) => UnsupportedOperError(other, op);

    #endregion

    #region OperateFiniteStaticSequence

    public abstract GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op);
    public GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Subst op) => UnsupportedOperError(other, op);
    public GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Mult op) => UnsupportedOperError(other, op);
    public GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Div op) => UnsupportedOperError(other, op);
    public GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Mod op) => UnsupportedOperError(other, op);
    public GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Power op) => UnsupportedOperError(other, op);
    public GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, LessTh op) => UnsupportedOperError(other, op);


    #endregion

    #region OperateInfiniteStaticSequence

    public abstract GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op);
    public GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Subst op) => UnsupportedOperError(other, op);
    public GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Mult op) => UnsupportedOperError(other, op);
    public GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Div op) => UnsupportedOperError(other, op);
    public GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Mod op) => UnsupportedOperError(other, op);
    public GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Power op) => UnsupportedOperError(other, op);
    public GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, LessTh op) => UnsupportedOperError(other, op);

    #endregion

    #region OperateGeneratorSequence

    public abstract GSObject OperateGeneratorSequence(GeneratorSequence other, Add op);

    public GSObject OperateGeneratorSequence(GeneratorSequence other, Subst op) => UnsupportedOperError(other, op);
    public GSObject OperateGeneratorSequence(GeneratorSequence other, Mult op) => UnsupportedOperError(other, op);
    public GSObject OperateGeneratorSequence(GeneratorSequence other, Div op) => UnsupportedOperError(other, op);
    public GSObject OperateGeneratorSequence(GeneratorSequence other, Mod op) => UnsupportedOperError(other, op);
    public GSObject OperateGeneratorSequence(GeneratorSequence other, Power op) => UnsupportedOperError(other, op);
    public GSObject OperateGeneratorSequence(GeneratorSequence other, LessTh op) => UnsupportedOperError(other, op);

    #endregion
}

