namespace GSharp.Objects.Figures;
using System;
using GSharp.Types;
using GSharp.Exceptions;

public class Ray : GeometricLocation
{
  public readonly Point FirstPoint;

  public readonly Point DirectorVector;

  public Ray()
  {
    (Point p1, Point p2) = Point.TwoDifferentPoints();

    this.FirstPoint = p1;
    this.DirectorVector = p2 - FirstPoint;
  }

  public Ray(Point FirstPoint, Point SecondPoint)
  {
    // if (Functions.EqualVectorsApprox(FirstPoint, SecondPoint))
    //     throw new RuntimeError(null, "Equal Points Cannot determine a Ray");

    this.FirstPoint = FirstPoint;
    this.DirectorVector = SecondPoint - FirstPoint;
  }

  public static Ray PointDirectorVec(Point Point, Point DirectionVector)
      => new(Point, Point + DirectionVector);

  public override Point Sample()
  {
    var length = rnd.RandDoubleRange(0, Math.Max(Figure.WindowEndX - Figure.WindowStartX, Figure.WindowEndY - Figure.WindowStartY));
    return this.FirstPoint + length*this.DirectorVector/this.DirectorVector.Norm;
  }
  public override string ToString() => $"Ray: [from: {this.FirstPoint} directorV: {this.DirectorVector}]";

  public override bool Equals(GSObject obj)
      => obj is Ray R &&
          Functions.EqualVectorsApprox(R.FirstPoint, this.FirstPoint) &&
          Functions.EqualApprox(0, this.DirectorVector.AngleTo(R.DirectorVector));

  public override string GetTypeName() => TypeName.Ray.ToString();

  public override bool SameTypeAs(GSObject gso) => gso is Ray;
  public override bool SameTypeAs(GSType gst) => gst.SameTypeAs(TypeName.Ray);
}