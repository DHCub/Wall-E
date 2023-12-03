namespace GSharp.Objects.Figures;

public abstract class GeometricLocation : Figure
{
  public override GSObject OperatePoint(Point other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperatePoint(Point other, Subst op) => UnsupportedOperError(other, op);
  public override GSObject OperatePoint(Point other, Mult op) => UnsupportedOperError(other, op);

  public override GSObject OperateMeasure(Measure other, Mult op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Div op) => UnsupportedOperError(other, op);


  public override GSObject OperateScalar(Scalar other, Mult op) => UnsupportedOperError(other, op);
  public override GSObject OperateScalar(Scalar other, Div op) => UnsupportedOperError(other, op);
}