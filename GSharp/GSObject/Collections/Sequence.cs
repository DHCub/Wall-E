using GSharp.Objects.Figures;

namespace GSharp.Objects.Collections;

public abstract class Sequence : GSObject
{
  protected const string INFINITE_SEQUENCE = "INFINITE SEQUENCE";
  public abstract Sequence GetRemainder(int start);
  public abstract GSObject this[int i] { get; }
  public abstract GSObject GSCount();
  public abstract IEnumerable<GSObject> GetPrefixValues();
  public override string GetTypeName() => SEQUENCE;

  public override GSObject OperateScalar(Scalar other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Subst op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Mult op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Div op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Mod op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, LessTh op) => UnsupportedOperError(other, op);

  public override GSObject OperateMeasure(Measure other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Subst op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Mult op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Div op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Mod op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, LessTh op) => UnsupportedOperError(other, op);


  public override GSObject OperatePoint(Point other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperatePoint(Point other, Subst op) => UnsupportedOperError(other, op);
  public override GSObject OperatePoint(Point other, Mult op) => UnsupportedOperError(other, op);

  public override GSObject OperateString(GSharp.Objects.String other, Add op) => UnsupportedOperError(other, op);
}