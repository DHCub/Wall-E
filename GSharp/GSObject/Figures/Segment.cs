using GSharp.Exceptions;

namespace GSharp.Objects.Figures;

using GSharp.Types;

public partial class Segment : GeometricLocation
{
  public readonly Point A_Point;
  public readonly Point B_Point;

  public Segment()
  {
    (Point p1, Point p2) = Point.TwoDifferentPoints();

    this.A_Point = p1;
    this.B_Point = p2;
  }

  public Segment(Point A_Point, Point B_Point)
  {
    if (Functions.Equal_Vectors_Approx(A_Point, B_Point))
      throw new RuntimeError(null, "Equal Points do not Determine a Segment");
    this.A_Point = A_Point;
    this.B_Point = B_Point;
  }

  public override Point Sample()
  {
    var Vector = this.B_Point - this.A_Point;

    var norm = Vector.Norm;
    var length = Figure.rnd.RandfRange(0, (float)norm);

    Vector = (length / norm) * Vector;

    return A_Point + Vector;
  }

  public override string GetTypeName() => TypeName.Segment.ToString();

  public override bool Equals(GSObject obj)
      => obj is Segment S && (
          Functions.Equal_Vectors_Approx(S.A_Point, this.A_Point) && Functions.Equal_Vectors_Approx(S.B_Point, this.B_Point) ||
          Functions.Equal_Vectors_Approx(S.A_Point, this.B_Point) && Functions.Equal_Vectors_Approx(S.B_Point, this.A_Point));

  public override string ToString() => $"Segment: [from: {this.A_Point} to: {this.B_Point}]";

}