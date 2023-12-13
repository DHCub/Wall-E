namespace GSharp.Objects.Figures;
using System;
using GSharp.Types;
using GSharp.Exceptions;
public class Point : Figure
{
  public readonly double XCoord;
  public readonly double YCoord;
  public double Norm
  {
    get
    {
      return Math.Sqrt(this.XCoord * this.XCoord + this.YCoord * this.YCoord);
    }
  }


  public override string ToString()
  {
    return $"({XCoord}, {YCoord})";
  }

  public Point() : this(
      Figure.rnd.RandDoubleRange(Figure.WindowStartX, Figure.WindowEndX),
      Figure.rnd.RandDoubleRange(Figure.WindowStartY, Figure.WindowEndY))
  { }

  public Point(double XCoord, double YCoord)
  {
    this.XCoord = XCoord;
    this.YCoord = YCoord;
  }

  public static Point operator -(Point A, Point B)
      => new(A.XCoord - B.XCoord, A.YCoord - B.YCoord);

  public static Point operator +(Point A, Point B)
      => new(A.XCoord + B.XCoord, A.YCoord + B.YCoord);

  public static Point operator *(double alpha, Point A)
      => new(A.XCoord * alpha, A.YCoord * alpha);

  public static Point operator *(Point A, double alpha)
      => new(A.XCoord * alpha, A.YCoord * alpha);

  public static Point operator /(Point P, double alpha)
      => new(P.XCoord / alpha, P.YCoord / alpha);

  public double DotProduct(Point other) => this.XCoord * other.XCoord + this.YCoord * other.YCoord;

  public bool IsColinear(Point other)
  {
    if (this.isOrigin() || other.isOrigin()) return true;
    var cos = this.DotProduct(other);
    var normMult = this.Norm * other.Norm;

    return Functions.EqualApprox(cos, -normMult) || Functions.EqualApprox(cos, normMult);
  }

  public bool isOrigin()
      => Functions.EqualApprox(this.XCoord, 0) && Functions.EqualApprox(this.YCoord, 0);

  public double AngleTo(Point other)
  {
    var cosTimesNorm = this.DotProduct(other);
    var sinTimesNorm = this.XCoord * other.YCoord - this.YCoord * other.XCoord;

    var angle = Math.Atan2(sinTimesNorm, cosTimesNorm);
    if (angle < 0) angle = angle + 2 * Math.PI;

    return angle;
  }

  public Point Orthogonal()
      => new(this.YCoord, -this.XCoord);

  public bool EqualApprox(Point other)
      => Functions.EqualVectorsApprox(this, other);

  public double DistanceTo(Point other)
      => Functions.Distance(this, other);

  public double DistanceTo(Line L)
      => Functions.Distance(this, L);

  public Point GetRotatedAsVector(double Angle)
  {
    var x2 = Math.Cos(Angle) * XCoord - Math.Sin(Angle) * YCoord;
    var y2 = Math.Sin(Angle) * XCoord + Math.Cos(Angle) * YCoord;

    return new(x2, y2);
  }

  public override Point Sample() => this;

  public static (Point p1, Point p2) TwoDifferentPoints()
  {
    Point p1 = new();
    Point p2 = new();

    if (!Functions.EqualVectorsApprox(p1, p2)) return (p1, p2);

    p2 = new Point(1E-7, 0);
    p2 = p2.GetRotatedAsVector(Figure.rnd.RandDoubleRange(0, (2 * Math.PI)));

    p2 = p1 + p2;

    return (p1, p2);
  }

  public override string GetTypeName() => TypeName.Point.ToString();

  public override bool Equals(GSObject obj) => obj is Point P && Functions.EqualVectorsApprox(this, P);

  public override bool SameTypeAs(GSObject gso) => gso is Point;
  public override bool SameTypeAs(GSType gst) => gst.SameTypeAs(TypeName.Point);

  public override GSObject OperateScalar(Scalar other, Mult op)
      => this * other.value;
  public override GSObject OperateScalar(Scalar other, Div op)
      => this / other.value;

  public override GSObject OperateScalar(Scalar other, Indexer op)
  {
    var (isInteger, i) = Functions.GetInteger(other);

    if (!isInteger || i < 0) throw new RuntimeError(null, IndexingValueMustBeNonNegativeInteger(other), null);
    if (i > 1) throw new RuntimeError(null, $"Points are two-dimensional, the {i+1}th coordinate of a point can therefore not be accessed", null);
    return (i == 0) ? new Scalar(this.XCoord) : new Scalar(this.YCoord);
  }


  public override GSObject OperateMeasure(Measure other, Div op)
      => this / other.value;
  public override GSObject OperateMeasure(Measure other, Mult op)
      => this * other.value;

  public override GSObject OperatePoint(Point other, Add op)
      => this + other;

  public override GSObject OperatePoint(Point other, Subst op)
      => this - other;


}