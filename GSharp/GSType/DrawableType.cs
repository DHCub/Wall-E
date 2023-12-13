namespace GSharp.Types;

public class DrawableType : GSType
{
  public override GSType Copy() => this;

  public override bool SameTypeAs(DrawableType drawableType)
      => true;
  public override bool SameTypeAs(FigureType figureType)
      => true;

  public override bool SameTypeAs(SequenceType sequenceType)
      => sequenceType.IsDrawable();

  public override bool SameTypeAs(SimpleType simpleType)
      => simpleType.IsDrawable();

  public override string ToString() => DRAWABLE;
  public override GSType GetMostRestrictedOrError(DrawableType drawableType, bool sameTypesChecked = false)
      => this;
  public override GSType GetMostRestrictedOrError(FigureType figureType, bool sameTypesChecked = false)
      => figureType.Copy();

  public override GSType GetMostRestrictedOrError(SequenceType sequenceType, bool sameTypesChecked = false)
  {
    if (!sameTypesChecked) if (!sequenceType.IsDrawable()) throw new System.Exception(MOST_RESTRICTED_ON_DIFFERENT_TYPES_ERROR);

    return new SequenceType(this.GetMostRestrictedOrError(sequenceType.MostRestrictedType, true));
  }

  public override GSType GetMostRestrictedOrError(SimpleType simpleType, bool sameTypesChecked = false)
      => simpleType.GetMostRestrictedOrError(this);

  public override bool IsDrawable() => true;
  public override bool IsFigure() => true;

  public override (GSType, string) OperableMeasure(Mult op) => (TypeName.Point, null);
  public override (GSType, string) OperableMeasure(Div op) => (TypeName.Point, null);
  public override (GSType, string) OperableMeasure(LessTh op) => UnsupportedOperator(TypeName.Measure.ToString(), op);
  public override (GSType, string) OperablePoint(Mult op) => UnsupportedOperator(TypeName.Point.ToString(), op);
  public override (GSType, string) OperableScalar(Div op) => (TypeName.Point, null);
  public override (GSType, string) OperableScalar(LessTh op) => UnsupportedOperator(TypeName.Scalar.ToString(), op);
  public override (GSType, string) OperableScalar(Mult op) => (TypeName.Point, null);
  // we could be indexing a point or a sequence of drawables
  public override (GSType, string) OperableScalar(Indexer op) => (new UndefinedType(), null);

  public override (GSType, string) OperableSequence(SequenceType other, Add op)
  {
    if (!this.SameTypeAs(other)) return (new UndefinedType(), $"Cannot Add Drawable and Sequence of type {other.ToString()}");

    return (this.GetMostRestrictedOrError(other), null);
  }
  public override (GSType, string) OperableUndefined(Add op) => (TypeName.Point, null);
  public override (GSType, string) OperableUndefined(Subst op) => (TypeName.Point, null);
  public override (GSType, string) OperableUndefined(Mult op) => (TypeName.Point, null);
  public override (GSType, string) OperableUndefined(Div op) => (TypeName.Point, null);
  public override (GSType, string) OperableUndefined(Mod op) => UnsupportedOperator(UNDEFINED, op);
  public override (GSType, string) OperableUndefined(LessTh op) => UnsupportedOperator(UNDEFINED, op);
  // we could be indexing a point or a sequence of drawables
  public override (GSType, string) OperableUndefined(Indexer op) => (new UndefinedType(), null);

}