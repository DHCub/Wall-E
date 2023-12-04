using GSharp.Objects.Collections;
using GSharp.Objects.Figures;
using GSharp.Types;
namespace GSharp.Objects;

public class String : GSObject
{
  public readonly string value;

  public String(string value)
  {
    this.value = value;
  }

  public override string ToString()
      => $"\"{this.value}\"";

  public override string GetTypeName()
      => TypeName.String.ToString();

  public override bool GetTruthValue()
      => true;

  public override bool Equals(GSObject obj) => obj is String s && s.value == this.value;

  public override bool SameTypeAs(GSObject gso) => gso is String;

  public override GSObject OperateString(String other, Add op) => new String(this.value + other.value);


  public override GSObject OperateScalar(Scalar other, Add op)
      => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Subst op)
      => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Mult op)
      => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Div op)
      => UnsupportedOperError(other, op);

  public override GSObject OperateScalar(Scalar other, Mod op)
      => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, LessTh op)
      => UnsupportedOperError(other, op);



  public override GSObject OperatePoint(Point other, Add op)
      => UnsupportedOperError(other, op);

  public override GSObject OperatePoint(Point other, Mult op)
      => UnsupportedOperError(other, op);

  public override GSObject OperatePoint(Point other, Subst op)
      => UnsupportedOperError(other, op);


  public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op)
      => UnsupportedOperError(other, op);

  public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op)
      => UnsupportedOperError(other, op);


  public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op)
      => UnsupportedOperError(other, op);

  public override GSObject OperateMeasure(Measure other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Subst op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Mult op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Div op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Mod op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, LessTh op) => UnsupportedOperError(other, op);

  public override GSObject OperateUndefined(Undefined other, Add op) => UnsupportedOperError(other, op);

}