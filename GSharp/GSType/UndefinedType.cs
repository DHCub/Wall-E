using System;
using GSharp.Objects;

namespace GSharp.Types;

public class UndefinedType : GSType
{
    public override GSType Copy() => this;

    public override bool SameTypeAs(SimpleType simpleType) => true;
    public override bool SameTypeAs(DrawableType drawableType) => true;
    public override bool SameTypeAs(FigureType figureType) => true;
    public override bool SameTypeAs(SequenceType sequenceType) => true;

    public override string ToString() => UNDEFINED;
    public override GSType GetMostRestrictedOrError(DrawableType drawableType, bool sameTypesChecked = false)
        => drawableType.Copy();
    public override GSType GetMostRestrictedOrError(FigureType figureType, bool sameTypesChecked = false)
        => figureType.Copy();

    public override GSType GetMostRestrictedOrError(SequenceType sequenceType, bool sameTypesChecked = false)
        => sequenceType.Copy();

    public override GSType GetMostRestrictedOrError(SimpleType simpleType, bool sameTypesChecked = false)
        => simpleType.Copy();

    public override bool IsDrawable() => true;
    public override bool IsFigure() => true;

    public override (GSType, string) OperableMeasure(Div op)
        => (this, null);
    public override (GSType, string) OperableMeasure(Mult op)
        => (this, null);
    public override (GSType, string) OperableMeasure(LessTh op)
        => (TypeName.Scalar, null);

    public override (GSType, string) OperablePoint(Mult op)
        => (TypeName.Point, null);

    public override (GSType, string) OperableScalar(Mult op)
        => (this, null);

    public override (GSType, string) OperableScalar(Div op)
        => (this, null);

    public override (GSType, string) OperableScalar(LessTh op)
        => (TypeName.Scalar, null);

    public override (GSType, string) OperableScalar(Indexer op)
        => (this, null);


    public override (GSType, string) OperableSequence(SequenceType other, Add op)
        => (other.Copy(), null);
    
    public override (GSType, string) OperableUndefined(Add op)
        => (this, null);
    public override (GSType, string) OperableUndefined(Subst op)
        => (this, null);
    public override (GSType, string) OperableUndefined(Mult op)
        => (this, null);
    public override (GSType, string) OperableUndefined(Div op)
        => (this, null);

    public override (GSType, string) OperableUndefined(Mod op)
        => (TypeName.Scalar, null); // little type inference

    public override (GSType, string) OperableUndefined(LessTh op)
        => (TypeName.Scalar, null);

    public override (GSType, string) OperableUndefined(Indexer op)
        => (this, null);
    
}