using System;
using GSharp.Objects.Collections;
using GSharp.Objects.Figures;
using GSharp.Types;

namespace GSharp.Objects;

public class Measure : GSObject
{
  public readonly double value;

  public Measure(double value)
  {
    this.value = Math.Abs(value);
  }

  public override bool Equals(GSObject obj)
  {
    if (obj is Measure measure)
    {
      return Functions.EqualApprox(Math.Abs(measure.value), this.value);
    }
    if (obj is Scalar scalar)
    {
      return Functions.EqualApprox(Math.Abs(scalar.value), this.value);
    }

    return false;
  }
  public override string GetTypeName() => TypeName.Measure.ToString();

  public override string ToString() => this.value.ToString();

  public override bool GetTruthValue() => this.value != 0;

  public override bool SameTypeAs(GSObject gso) => gso is Measure;

  public override bool SameTypeAs(GSType gst) => gst.SameTypeAs(TypeName.Measure);

  public override GSObject OperateScalar(Scalar other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Subst op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Mult op) => new Measure(this.value * Math.Floor(other.value));
  public override GSObject OperateScalar(Scalar other, Div op) => new Measure(this.value / Math.Floor(other.value));
  public override GSObject OperateScalar(Scalar other, Mod op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, LessTh op) => new Scalar(Functions.LessThanApprox(this.value, Math.Abs(other.value)));
  public override GSObject OperateScalar(Scalar other, Indexer op) => UnsupportedOperError(other, op);


  public override GSObject OperateMeasure(Measure other, Add op) => new Measure(this.value + other.value);
  public override GSObject OperateMeasure(Measure other, Subst op) => new Measure(this.value - other.value);
  public override GSObject OperateMeasure(Measure other, Mult op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Div op) => new Scalar(Math.Floor(this.value / other.value));
  public override GSObject OperateMeasure(Measure other, LessTh op) => new Scalar(Functions.LessThanApprox(this.value, other.value));
  public override GSObject OperateMeasure(Measure other, Mod op) => UnsupportedOperError(other, op);

  public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op) => UnsupportedOperError(other, op);


  public override GSObject OperatePoint(Point other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperatePoint(Point other, Subst op) => UnsupportedOperError(other, op);
  public override GSObject OperatePoint(Point other, Mult op) => this.value * other;

  public override GSObject OperateUndefined(Undefined other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperateString(String other, Add op) => UnsupportedOperError(other, op);


}