
namespace GSharp.Types;

public class FigureType : GSType
{
  public override GSType Copy() => this;
  public override GSType GetMostRestrictedOrError(DrawableType drawableType, bool sameTypesChecked = false)
      => this;
  public override GSType GetMostRestrictedOrError(FigureType figureType, bool sameTypesChecked = false)
      => this;

  public override GSType GetMostRestrictedOrError(SequenceType sequenceType, bool sameTypesChecked = false)
      => throw new System.Exception(MOST_RESTRICTED_ON_DIFFERENT_TYPES_ERROR);
  public override GSType GetMostRestrictedOrError(SimpleType simpleType, bool sameTypesChecked = false)
      => simpleType.GetMostRestrictedOrError(this, sameTypesChecked);
  public override bool IsDrawable() => true;
  public override bool IsFigure() => true;


  public override (GSType, string) OperablePoint(Mult op) => UnsupportedOperator(FIGURE, op);

  public override (GSType, string) OperableMeasure(Mult op) => (TypeName.Point, null);
  public override (GSType, string) OperableMeasure(Div op) => (TypeName.Point, null);
  public override (GSType, string) OperableMeasure(LessTh op) => UnsupportedOperator(FIGURE, op);

  public override (GSType, string) OperableScalar(Mult op) => (TypeName.Point, null);
  public override (GSType, string) OperableScalar(Div op) => (TypeName.Point, null);
  public override (GSType, string) OperableScalar(LessTh op) => UnsupportedOperator(FIGURE, op);
  // if we are indexing a figure, we must be indexing a point, therefore we must be returning a coordinate
  public override (GSType, string) OperableScalar(Indexer op) => (TypeName.Scalar, null);

  public override (GSType, string) OperableSequence(SequenceType other, Add op) => UnsupportedOperator(FIGURE, op);


  public override (GSType, string) OperableUndefined(Add op) => (TypeName.Point, null);
  public override (GSType, string) OperableUndefined(Subst op) => (TypeName.Point, null);
  public override (GSType, string) OperableUndefined(Mult op) => (TypeName.Point, null);
  public override (GSType, string) OperableUndefined(Div op) => (TypeName.Point, null);
  public override (GSType, string) OperableUndefined(Mod op) => UnsupportedOperator(FIGURE, op);
  public override (GSType, string) OperableUndefined(LessTh op) => UnsupportedOperator(FIGURE, op);
  // if we are indexing a figure, we must be indexing a point, therefore we must be returning a coordinate
  public override (GSType, string) OperableUndefined(Indexer op) => (TypeName.Scalar, null);


  public override bool SameTypeAs(DrawableType drawableType) => true;
  public override bool SameTypeAs(FigureType figureType) => true;
  public override bool SameTypeAs(SequenceType sequenceType) => false;
  public override bool SameTypeAs(SimpleType simpleType) => simpleType.IsFigure();
  public override string ToString() => FIGURE;


}