namespace GSharp.Types;

public class SimpleType : GSType
{
  public readonly TypeName type;

  public SimpleType(TypeName type)
  {
    this.type = type;
  }

  public static implicit operator SimpleType(TypeName type) => new(type);

  public override GSType Copy() => this;

  public override string ToString() => type.ToString();
  public override bool IsFigure()
      => type switch
      {
        TypeName.Point or TypeName.Line or TypeName.Ray or TypeName.Segment or TypeName.Circle or TypeName.Arc => true,
        _ => false
      };

  public override bool IsDrawable() => IsFigure();

  public override bool SameTypeAs(DrawableType drawableType)
      => this.IsDrawable();

  public override bool SameTypeAs(FigureType figureType)
      => this.IsFigure();

  public override bool SameTypeAs(SequenceType sequenceType)
      => false;

  public override bool SameTypeAs(SimpleType simpleType)
      => this.type == simpleType.type;

  public override GSType GetMostRestrictedOrError(DrawableType drawableType, bool sameTypesChecked = false)
  {
    if (this.IsDrawable()) return this;

    throw new System.Exception(MOST_RESTRICTED_ON_DIFFERENT_TYPES_ERROR);
  }

  public override GSType GetMostRestrictedOrError(FigureType figureType, bool sameTypesChecked = false)
  {
    if (this.IsFigure()) return this;

    throw new System.Exception(MOST_RESTRICTED_ON_DIFFERENT_TYPES_ERROR);
  }

  public override GSType GetMostRestrictedOrError(SequenceType sequenceType, bool sameTypesChecked = false)
      => throw new System.Exception(MOST_RESTRICTED_ON_DIFFERENT_TYPES_ERROR);

  public override GSType GetMostRestrictedOrError(SimpleType simpleType, bool sameTypesChecked = false)
      => simpleType.type == this.type ? this : throw new System.Exception(MOST_RESTRICTED_ON_DIFFERENT_TYPES_ERROR);

  public override (GSType, string) OperableMeasure(Mult op)
      => type switch
      {
        TypeName.Scalar => (TypeName.Measure, null),
        TypeName.Point => (TypeName.Point, null),
        _ => UnsupportedOperator(TypeName.Measure.ToString(), op)
      };

  public override (GSType, string) OperableMeasure(LessTh op)
      => type switch
      {
        TypeName.Scalar or TypeName.Scalar => (TypeName.Scalar, null),
        _ => UnsupportedOperator(TypeName.Measure.ToString(), op)
      };


  public override (GSType, string) OperableScalar(Mult op)
      => type switch
      {
        TypeName.Scalar => (TypeName.Scalar, null),
        TypeName.Point => (TypeName.Point, null),
        TypeName.Measure => (TypeName.Measure, null),
        _ => UnsupportedOperator(TypeName.Scalar.ToString(), op)
      };


  public override (GSType, string) OperableScalar(Div op)
      => type switch
      {
        TypeName.Scalar => (TypeName.Scalar, null),
        TypeName.Point => (TypeName.Point, null),
        TypeName.Measure => (TypeName.Measure, null),
        _ => UnsupportedOperator(TypeName.Scalar.ToString(), op)
      };

  public override (GSType, string) OperableScalar(LessTh op)
      => type switch
      {
        TypeName.Scalar or TypeName.Measure => (TypeName.Scalar, null),
        _ => UnsupportedOperator(TypeName.Scalar.ToString(), op)
      };

  public override (GSType, string) OperablePoint(Mult op)
      => type switch
      {
        TypeName.Scalar or TypeName.Measure => (TypeName.Point, null),
        _ => UnsupportedOperator(TypeName.Point.ToString(), op)
      };

  public override (GSType, string) OperableSequence(SequenceType other, Add op) => UnsupportedOperator(SEQUENCE, op);

  public override (GSType, string) OperableUndefined(Add op)
      => type switch
      {
        TypeName.Point or TypeName.Scalar or TypeName.Measure => (type, null),
        _ => UnsupportedOperator(UNDEFINED, op)
      };

  public override (GSType, string) OperableUndefined(Subst op)
      => type switch
      {
        TypeName.Point or TypeName.Scalar or TypeName.Measure => (type, null),
        _ => UnsupportedOperator(UNDEFINED, op)
      };

  public override (GSType, string) OperableUndefined(Mult op)
      => type switch
      {
        TypeName.Point or TypeName.Scalar or TypeName.Measure => (type, null),
        _ => UnsupportedOperator(UNDEFINED, op)
      };

  public override (GSType, string) OperableUndefined(Div op)
      => type switch
      {
        TypeName.Point or TypeName.Scalar => (type, null),
        TypeName.Measure => (new UndefinedType(), null),
        _ => UnsupportedOperator(UNDEFINED, op)
      };

  public override (GSType, string) OperableUndefined(LessTh op)
      => type switch
      {
        TypeName.Measure or TypeName.Scalar => (TypeName.Scalar, null),
        _ => UnsupportedOperator(UNDEFINED, op)
      };

  public override (GSType, string) OperableUndefined(Mod op)
      => type == TypeName.Scalar ? (TypeName.Scalar, null) : UnsupportedOperator(UNDEFINED, op);



}