namespace GSharp.Objects.Figures;
using System;
using GSharp.Types;
using GSharp.Exceptions;

public partial class Segment : GeometricLocation
{
  public readonly Point APoint;
  public readonly Point BPoint;

  public Segment()
  {
    (Point p1, Point p2) = Point.TwoDifferentPoints();

    this.APoint = p1;
    this.BPoint = p2;
  }

  public Segment(Point APoint, Point BPoint)
  {
    this.APoint = APoint;
    this.BPoint = BPoint;
  }

  public override Point Sample()
  {
    var Vector = this.BPoint - this.APoint;

    var norm = Vector.Norm;
    var length = Figure.rnd.RandDoubleRange(0, norm);

    Vector = (length / norm) * Vector;

    return APoint + Vector;
  }

  public override string GetTypeName() => TypeName.Segment.ToString();

  public override bool Equals(GSObject obj)
      => obj is Segment S && (
          Functions.EqualVectorsApprox(S.APoint, this.APoint) && Functions.EqualVectorsApprox(S.BPoint, this.BPoint) ||
          Functions.EqualVectorsApprox(S.APoint, this.BPoint) && Functions.EqualVectorsApprox(S.BPoint, this.APoint));

    public override bool SameTypeAs(GSObject gso) => gso is Segment;
    public override bool SameTypeAs(GSType gst) => gst.SameTypeAs(TypeName.Segment);
    public override string ToString() => $"Segment: [from: {this.APoint} to: {this.BPoint}]";

}