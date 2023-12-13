using System;
using GSharp.Expression;

namespace GSharp.Types;

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

public abstract class GSType : IOperable<Add>,
                               IOperable<Subst>,
                               IOperable<Mult>,
                               IOperable<Div>,
                               IOperable<Mod>,
                               IOperable<Power>,
                               IOperable<LessTh>,
                               IOperable<Indexer>
{

    protected const string SEQUENCE = "Sequence";
    protected const string UNDEFINED = "Undefined";
    protected const string DRAWABLE = "Drawable";
    protected const string FIGURE = "Figure";

    protected const string MOST_RESTRICTED_ON_DIFFERENT_TYPES_ERROR = "Cannot Find most restricted of two different types";
    public bool SameTypeAs(GSType other)
    {
        if (other is DrawableType DT) return this.SameTypeAs(DT);
        if (other is FigureType FT) return this.SameTypeAs(FT);
        if (other is SequenceType ST) return this.SameTypeAs(ST);
        if (other is SimpleType SmT) return this.SameTypeAs(SmT);
        if (other is UndefinedType) return true;

        throw new NotImplementedException("UNSUPPORTED GSTYPE");
    }

    public abstract bool SameTypeAs(FigureType figureType);
    public abstract bool SameTypeAs(DrawableType drawableType);
    public abstract bool SameTypeAs(SequenceType sequenceType);
    public abstract bool SameTypeAs(SimpleType simpleType);

    public GSType GetMostRestrictedOrError(GSType other, bool sameTypesChecked = false)
    {
        if (other is DrawableType DT) return this.GetMostRestrictedOrError(DT, sameTypesChecked);
        if (other is FigureType FT) return this.GetMostRestrictedOrError(FT, sameTypesChecked);
        if (other is SequenceType ST) return this.GetMostRestrictedOrError(ST, sameTypesChecked);
        if (other is SimpleType SmT) return this.GetMostRestrictedOrError(SmT, sameTypesChecked);
        if (other is UndefinedType) return this.Copy();

        throw new NotImplementedException("UNSUPPORTED GSTYPE");
    }

    public abstract GSType GetMostRestrictedOrError(FigureType figureType, bool sameTypesChecked = false);
    public abstract GSType GetMostRestrictedOrError(DrawableType drawableType, bool sameTypesChecked = false);
    public abstract GSType GetMostRestrictedOrError(SequenceType sequenceType, bool sameTypesChecked = false);
    public abstract GSType GetMostRestrictedOrError(SimpleType simpleType, bool sameTypesChecked = false);
    public abstract override string ToString();
    public abstract bool IsFigure();
    public abstract bool IsDrawable();
    public abstract GSType Copy();

    public static implicit operator GSType(TypeName type) => new SimpleType(type);


    #region Errors

    private static string CannotOperate(string operation, string T1, string T2)
    {
        string ERROR = $"Cannot {operation} ";
        if (T1 is UNDEFINED) return ERROR + T2;
        if (T2 is UNDEFINED) return ERROR + T1;

        return ERROR + $"{T1} and {T2}";
    }

    private static string CannotElevate(string T1, string T2)
    {
        if (T1 == UNDEFINED) return $"{T2} cannot be an exponent";
        if (T2 == UNDEFINED) return $"{T1} cannot be raised to a power";

        return $"Cannot raise {T1} to the power of {T2}";
    }

    private static string CannotFindModuloOf(string T1, string T2)
    {
        string ERROR = "Modulo Operator is not defined for ";
        if (T1 == UNDEFINED) return ERROR + T2;
        if (T2 == UNDEFINED) return ERROR + T1;

        return $"Cannot find modulo of {T1} over {T2}";
    }

    private static string NoOrderRelation(string T1, string T2)
    {
        string IS_NOT_COMPARABLE = " is not Comparable";
        if (T2 == UNDEFINED) return T1 + IS_NOT_COMPARABLE;
        if (T1 == UNDEFINED) return T2 + IS_NOT_COMPARABLE;

        return $"Cannot Compare {T1} and {T2}";
    }

    private static string CannotIndex(string T1, string T2)
    {
        string CANNOT_INDEX = "Cannot Index ";
        if (T1 == UNDEFINED) return T2 + " is not a valid indexer type";
        if (T2 == UNDEFINED) return CANNOT_INDEX + T1;

        return CANNOT_INDEX + T1 + " with indexer type " + T2;
    }


    public (GSType, string) UnsupportedOperator(string otherT, Add op) => (new UndefinedType(), CannotOperate("Add", this.ToString(), otherT));

    public (GSType, string?) UnsupportedOperator(string otherT, Subst op) => (new UndefinedType(), CannotOperate("Substract", this.ToString(), otherT));
    public (GSType, string?) UnsupportedOperator(string otherT, Mult op) => (new UndefinedType(), CannotOperate("Multiply", this.ToString(), otherT));
    public (GSType, string?) UnsupportedOperator(string otherT, Div op) => (new UndefinedType(), CannotOperate("Divide", this.ToString(), otherT));
    public (GSType, string?) UnsupportedOperator(string otherT, Power op) => (new UndefinedType(), CannotElevate(this.ToString(), otherT));
    public (GSType, string?) UnsupportedOperator(string otherT, Mod op) => (new UndefinedType(), CannotFindModuloOf(this.ToString(), otherT));
    public (GSType, string?) UnsupportedOperator(string otherT, LessTh op) => (new UndefinedType(), NoOrderRelation(this.ToString(), otherT));
    public (GSType, string?) UnsupportedOperator(string otherT, Indexer op) => (new UndefinedType(), CannotIndex(this.ToString(), otherT));
   
    #endregion

    #region OperablePoint
    
    public (GSType, string?) OperablePoint(Add op) => this.SameTypeAs(TypeName.Point)? (TypeName.Point, null) : UnsupportedOperator(TypeName.Point.ToString(), op);
    public (GSType, string?) OperablePoint(Subst op) => this.SameTypeAs(TypeName.Point)? (TypeName.Point, null) : UnsupportedOperator(TypeName.Point.ToString(), op);
    public abstract (GSType, string?) OperablePoint(Mult op);
    public (GSType, string?) OperablePoint(Div op) => UnsupportedOperator(TypeName.Point.ToString(), op);
    public (GSType, string?) OperablePoint(Mod op) => UnsupportedOperator(TypeName.Point.ToString(), op);
    public (GSType, string?) OperablePoint(Power op) => UnsupportedOperator(TypeName.Point.ToString(), op);
    public (GSType, string?) OperablePoint(LessTh op) => UnsupportedOperator(TypeName.Point.ToString(), op);
    public (GSType, string?) OperablePoint(Indexer op) => UnsupportedOperator(TypeName.Point.ToString(), op);

    #endregion

    #region OperableLine

    public (GSType, string?) OperableLine(Add op) => UnsupportedOperator(TypeName.Line.ToString(), op);
    public (GSType, string?) OperableLine(Subst op) => UnsupportedOperator(TypeName.Line.ToString(), op);
    public (GSType, string?) OperableLine(Mult op) => UnsupportedOperator(TypeName.Line.ToString(), op);
    public (GSType, string?) OperableLine(Div op) => UnsupportedOperator(TypeName.Line.ToString(), op);
    public (GSType, string?) OperableLine(Mod op) => UnsupportedOperator(TypeName.Line.ToString(), op);
    public (GSType, string?) OperableLine(Power op) => UnsupportedOperator(TypeName.Line.ToString(), op);
    public (GSType, string?) OperableLine(LessTh op) => UnsupportedOperator(TypeName.Line.ToString(), op);
    public (GSType, string?) OperableLine(Indexer op) => UnsupportedOperator(TypeName.Line.ToString(), op);

    #endregion

    #region OperableSegment

    public (GSType, string?) OperableSegment(Add op) => UnsupportedOperator(TypeName.Segment.ToString(), op);
    public (GSType, string?) OperableSegment(Subst op) => UnsupportedOperator(TypeName.Segment.ToString(), op);
    public (GSType, string?) OperableSegment(Mult op) => UnsupportedOperator(TypeName.Segment.ToString(), op);
    public (GSType, string?) OperableSegment(Div op) => UnsupportedOperator(TypeName.Segment.ToString(), op);
    public (GSType, string?) OperableSegment(Mod op) => UnsupportedOperator(TypeName.Segment.ToString(), op);
    public (GSType, string?) OperableSegment(Power op) => UnsupportedOperator(TypeName.Segment.ToString(), op);
    public (GSType, string?) OperableSegment(LessTh op) => UnsupportedOperator(TypeName.Segment.ToString(), op);
    public (GSType, string?) OperableSegment(Indexer op) => UnsupportedOperator(TypeName.Segment.ToString(), op);


    #endregion

    #region OperableRay

    public (GSType, string?) OperableRay(Add op) => UnsupportedOperator(TypeName.Ray.ToString(), op);
    public (GSType, string?) OperableRay(Subst op) => UnsupportedOperator(TypeName.Ray.ToString(), op);
    public (GSType, string?) OperableRay(Mult op) => UnsupportedOperator(TypeName.Ray.ToString(), op);
    public (GSType, string?) OperableRay(Div op) => UnsupportedOperator(TypeName.Ray.ToString(), op);
    public (GSType, string?) OperableRay(Mod op) => UnsupportedOperator(TypeName.Ray.ToString(), op);
    public (GSType, string?) OperableRay(Power op) => UnsupportedOperator(TypeName.Ray.ToString(), op);
    public (GSType, string?) OperableRay(LessTh op) => UnsupportedOperator(TypeName.Ray.ToString(), op);
    public (GSType, string?) OperableRay(Indexer op) => UnsupportedOperator(TypeName.Ray.ToString(), op);
    

    #endregion

    #region OperableCircle

    public (GSType, string?) OperableCircle(Add op) => UnsupportedOperator(TypeName.Circle.ToString(), op);
    public (GSType, string?) OperableCircle(Subst op) => UnsupportedOperator(TypeName.Circle.ToString(), op);
    public (GSType, string?) OperableCircle(Mult op) => UnsupportedOperator(TypeName.Circle.ToString(), op);
    public (GSType, string?) OperableCircle(Div op) => UnsupportedOperator(TypeName.Circle.ToString(), op);
    public (GSType, string?) OperableCircle(Mod op) => UnsupportedOperator(TypeName.Circle.ToString(), op);
    public (GSType, string?) OperableCircle(Power op) => UnsupportedOperator(TypeName.Circle.ToString(), op);
    public (GSType, string?) OperableCircle(LessTh op) => UnsupportedOperator(TypeName.Circle.ToString(), op);
    public (GSType, string?) OperableCircle(Indexer op) => UnsupportedOperator(TypeName.Circle.ToString(), op);

    #endregion

    #region OperableArc

    public (GSType, string?) OperableArc(Add op) => UnsupportedOperator(TypeName.Arc.ToString(), op);
    public (GSType, string?) OperableArc(Subst op) => UnsupportedOperator(TypeName.Arc.ToString(), op);
    public (GSType, string?) OperableArc(Mult op) => UnsupportedOperator(TypeName.Arc.ToString(), op);
    public (GSType, string?) OperableArc(Div op) => UnsupportedOperator(TypeName.Arc.ToString(), op);
    public (GSType, string?) OperableArc(Mod op) => UnsupportedOperator(TypeName.Arc.ToString(), op);
    public (GSType, string?) OperableArc(Power op) => UnsupportedOperator(TypeName.Arc.ToString(), op);
    public (GSType, string?) OperableArc(LessTh op) => UnsupportedOperator(TypeName.Arc.ToString(), op);
    public (GSType, string?) OperableArc(Indexer op) => UnsupportedOperator(TypeName.Arc.ToString(), op);


    #endregion

    #region OperableScalar

    public (GSType, string?) OperableScalar(Add op) => this.SameTypeAs(TypeName.Scalar) ? (TypeName.Scalar, null) : UnsupportedOperator(TypeName.Scalar.ToString(), op);
    public (GSType, string?) OperableScalar(Subst op) => this.SameTypeAs(TypeName.Scalar) ? (TypeName.Scalar, null) : UnsupportedOperator(TypeName.Scalar.ToString(), op);
    public abstract (GSType, string?) OperableScalar(Mult op);
    public abstract (GSType, string?) OperableScalar(Div op);
    public (GSType, string?) OperableScalar(Mod op) => this.SameTypeAs(TypeName.Scalar) ? (TypeName.Scalar, null) : UnsupportedOperator(TypeName.Scalar.ToString(), op);
    public (GSType, string?) OperableScalar(Power op) => this.SameTypeAs(TypeName.Scalar) ? (TypeName.Scalar, null) : UnsupportedOperator(TypeName.Scalar.ToString(), op);
    public abstract (GSType, string?) OperableScalar(LessTh op);
    public abstract (GSType, string?) OperableScalar(Indexer op);


    #endregion

    #region OperableMeasure

    public (GSType, string?) OperableMeasure(Add op) => this.SameTypeAs(TypeName.Measure)? (TypeName.Measure, null) : UnsupportedOperator(TypeName.Measure.ToString(), op);
    public (GSType, string?) OperableMeasure(Subst op) => this.SameTypeAs(TypeName.Measure)? (TypeName.Measure, null) : UnsupportedOperator(TypeName.Measure.ToString(), op);
    public abstract (GSType, string?) OperableMeasure(Mult op);
    public abstract (GSType, string?) OperableMeasure(Div op);
    public (GSType, string?) OperableMeasure(Mod op) => UnsupportedOperator(TypeName.Measure.ToString(), op);
    public (GSType, string?) OperableMeasure(Power op) => UnsupportedOperator(TypeName.Measure.ToString(), op);
    public abstract (GSType, string?) OperableMeasure(LessTh op);
    public (GSType, string?) OperableMeasure(Indexer op) => UnsupportedOperator(TypeName.Measure.ToString(), op);



    #endregion

    #region OperableString

    public (GSType, string?) OperableString(Add op) => this.SameTypeAs(TypeName.String)? (TypeName.String, null) : UnsupportedOperator(TypeName.String.ToString(), op);
    public (GSType, string?) OperableString(Subst op) => UnsupportedOperator(TypeName.String.ToString(), op);
    public (GSType, string?) OperableString(Mult op) => UnsupportedOperator(TypeName.String.ToString(), op);
    public (GSType, string?) OperableString(Div op) => UnsupportedOperator(TypeName.String.ToString(), op);
    public (GSType, string?) OperableString(Mod op) => UnsupportedOperator(TypeName.String.ToString(), op);
    public (GSType, string?) OperableString(Power op) => UnsupportedOperator(TypeName.String.ToString(), op);
    public (GSType, string?) OperableString(LessTh op) => UnsupportedOperator(TypeName.String.ToString(), op);
    public (GSType, string?) OperableString(Indexer op) => UnsupportedOperator(TypeName.String.ToString(), op);

    #endregion

    #region OperableSequence

    public abstract (GSType, string?) OperableSequence(SequenceType other, Add op);
    public (GSType, string?) OperableSequence(SequenceType other, Subst op) => UnsupportedOperator(SEQUENCE, op);
    public (GSType, string?) OperableSequence(SequenceType other, Mult op) => UnsupportedOperator(SEQUENCE, op);
    public (GSType, string?) OperableSequence(SequenceType other, Div op) => UnsupportedOperator(SEQUENCE, op);
    public (GSType, string?) OperableSequence(SequenceType other, Mod op) => UnsupportedOperator(SEQUENCE, op);
    public (GSType, string?) OperableSequence(SequenceType other, Power op) => UnsupportedOperator(SEQUENCE, op);
    public (GSType, string?) OperableSequence(SequenceType other, LessTh op) => UnsupportedOperator(SEQUENCE, op);
    public (GSType, string?) OperableSequence(SequenceType other, Indexer op) => UnsupportedOperator(SEQUENCE, op);

    #endregion

    #region OperableUndefined

    public abstract (GSType, string?) OperableUndefined(Add op);
    public abstract (GSType, string?) OperableUndefined(Subst op);
    public abstract (GSType, string?) OperableUndefined(Mult op);
    public abstract (GSType, string?) OperableUndefined(Div op);
    public abstract (GSType, string?) OperableUndefined(Mod op);
    public (GSType, string?) OperableUndefined(Power op) => this.SameTypeAs(TypeName.Scalar) ? (TypeName.Scalar, null) : UnsupportedOperator(TypeName.Scalar.ToString(), op);
    public abstract (GSType, string?) OperableUndefined(LessTh op);
    public abstract (GSType, string?) OperableUndefined(Indexer op);

    #endregion

    #region OperableDrawable

    public (GSType, string?) OperableDrawable(Add op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableDrawable(Subst op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableDrawable(Mult op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableDrawable(Div op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableDrawable(Mod op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableDrawable(Power op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableDrawable(LessTh op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableDrawable(Indexer op) => UnsupportedOperator(DRAWABLE, op);

    #endregion

    #region OperableFigure

    public (GSType, string?) OperableFigure(Add op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableFigure(Subst op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableFigure(Mult op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableFigure(Div op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableFigure(Mod op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableFigure(Power op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableFigure(LessTh op) => UnsupportedOperator(DRAWABLE, op);
    public (GSType, string?) OperableFigure(Indexer op) => UnsupportedOperator(DRAWABLE, op);

    #endregion

}