using System;

namespace GSharp.Types;

public class SequenceType : GSType
{
    public GSType MostRestrictedType {get; private set;}

    public SequenceType(GSType initializer = null)
    {
        if (initializer == null) MostRestrictedType = new UndefinedType();
        else MostRestrictedType = initializer;
    }

    public override bool SameTypeAs(DrawableType drawableType) => this.IsDrawable();

    public override bool SameTypeAs(FigureType figureType) => false;
    public override bool SameTypeAs(SequenceType sequenceType) => this.MostRestrictedType.SameTypeAs(sequenceType.MostRestrictedType);
    public override bool SameTypeAs(SimpleType simpleType) => false;

    public override GSType GetMostRestrictedOrError(DrawableType drawableType, bool sameTypesChecked = false)
    {
        if (sameTypesChecked) return this.Copy();
        return (this.IsDrawable()) ? this.Copy() : throw new Exception(MOST_RESTRICTED_ON_DIFFERENT_TYPES_ERROR);
    }

    public override GSType GetMostRestrictedOrError(FigureType figureType, bool sameTypesChecked = false)
        => throw new Exception(MOST_RESTRICTED_ON_DIFFERENT_TYPES_ERROR);

    public override GSType GetMostRestrictedOrError(SequenceType sequenceType, bool sameTypesChecked = false)
    {
        if (!sameTypesChecked && !this.SameTypeAs(sequenceType)) return null;

        return new SequenceType(this.MostRestrictedType.GetMostRestrictedOrError(sequenceType.MostRestrictedType, true));
    }

    public override GSType GetMostRestrictedOrError(SimpleType simpleType, bool sameTypesChecked = false)
        => throw new Exception(MOST_RESTRICTED_ON_DIFFERENT_TYPES_ERROR);
    
    public override bool IsFigure() => false;
    public override bool IsDrawable() => this.MostRestrictedType.IsDrawable();

    public override string ToString() => $"Seq<{this.MostRestrictedType.ToString()}>";


    public (bool accepted, string? errorMessage) AcceptNewType(GSType newItem)
    {
        string errorMessage() => $"Seq<{MostRestrictedType.ToString()}> cannot contain {newItem.ToString()}";

        var accepted = MostRestrictedType.SameTypeAs(newItem);

        if (!accepted) return (false, errorMessage());
        
        MostRestrictedType = this.MostRestrictedType.GetMostRestrictedOrError(newItem, true);

        return (true, null);
    }

    public override GSType Copy() => new SequenceType(this.MostRestrictedType.Copy());


    private string CannotAddSequencesOfDifferentTypes(SequenceType other)
        => $"Cannot Add Sequences of type {this.ToString()} and {other.ToString()}";

    public override (GSType, string) OperableSequence(SequenceType other, Add op)
        => this.SameTypeAs(other) ? 
            (this.GetMostRestrictedOrError(other), null) : 
            (new SequenceType(new UndefinedType()), CannotAddSequencesOfDifferentTypes(other));


    public override (GSType, string) OperableMeasure(Mult op) => UnsupportedOperator(TypeName.Measure.ToString(), op);
    public override (GSType, string) OperableMeasure(Div op) => UnsupportedOperator(TypeName.Measure.ToString(), op);
    public override (GSType, string) OperableMeasure(LessTh op) => UnsupportedOperator(TypeName.Measure.ToString(), op);


    public override (GSType, string) OperablePoint(Mult op) => UnsupportedOperator(TypeName.Point.ToString(), op);

    public override (GSType, string) OperableUndefined(Add op) => (this.Copy(), null);
    public override (GSType, string) OperableUndefined(Subst op) => UnsupportedOperator(UNDEFINED, op);
    public override (GSType, string) OperableUndefined(Mult op) => UnsupportedOperator(UNDEFINED, op);
    public override (GSType, string) OperableUndefined(Div op) => UnsupportedOperator(UNDEFINED, op);
    public override (GSType, string) OperableUndefined(Mod op) => UnsupportedOperator(UNDEFINED, op);
    public override (GSType, string) OperableUndefined(LessTh op) => UnsupportedOperator(UNDEFINED, op);
    public override (GSType, string) OperableUndefined(Indexer op) => (this.MostRestrictedType, null);


    public override (GSType, string) OperableScalar(Mult op) => UnsupportedOperator(TypeName.Scalar.ToString(), op);
    public override (GSType, string) OperableScalar(Div op) => UnsupportedOperator(TypeName.Scalar.ToString(), op);
    public override (GSType, string) OperableScalar(LessTh op) => UnsupportedOperator(TypeName.Scalar.ToString(), op);
    public override (GSType, string) OperableScalar(Indexer op) => (this.MostRestrictedType, null);


}