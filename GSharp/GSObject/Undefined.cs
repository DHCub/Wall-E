using GSharp.Objects.Collections;
using GSharp.Objects.Figures;

namespace GSharp.Objects;

public class Undefined : GSObject
{

  public override bool Equals(GSObject obj) => obj is Undefined;

  public override bool GetTruthValue() => false;

  public override string GetTypeName() => UNDEFINED;

  public override string ToString() => UNDEFINED;

  public override bool SameTypeAs(GSObject gso) => gso is Undefined;

  public override GSObject OperateString(String other, Add op) => UnsupportedOperError(other, op);



  public override GSObject OperateScalar(Scalar other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Subst op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Mult op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Div op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Mod op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, LessTh op) => UnsupportedOperError(other, op);


  public override GSObject OperatePoint(Point other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperatePoint(Point other, Subst op) => UnsupportedOperError(other, op);
  public override GSObject OperatePoint(Point other, Mult op) => UnsupportedOperError(other, op);


  public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op) => new InfiniteStaticSequence();
  public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op) => new InfiniteStaticSequence();
  public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op) => new InfiniteStaticSequence();

  public override GSObject OperateMeasure(Measure other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Subst op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Mult op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Div op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Mod op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, LessTh op) => UnsupportedOperError(other, op);

  public override GSObject OperateUndefined(Undefined other, Add op) => UnsupportedOperError(other, op);
}

