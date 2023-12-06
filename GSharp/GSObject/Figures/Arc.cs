namespace GSharp.Objects.Figures;

using System;
using GSharp.Objects.Collections;
using GSharp.Types;


public class Arc : GeometricLocation
{
  public readonly Ray Start_Ray;

  public readonly Point Center;

  public readonly double Radius;
  public readonly double Angle;

  public Arc()
  {
    var core = new Circle();

    this.Start_Ray = new Ray(core.Center, new Point());
    this.Angle = Figure.rnd.RandDoubleRange(0, 2 * Math.PI);
    this.Center = core.Center;
    this.Radius = core.Radius;
  }

  public Arc(Ray Start_Ray, Ray End_Ray, double Radius)
  {
    this.Start_Ray = Start_Ray;
    this.Center = Start_Ray.FirstPoint;
    this.Radius = Radius;

    Angle = Start_Ray.DirectorVector.AngleTo(End_Ray.DirectorVector);
  }

  public Arc(Point Center, Point A, Point B, double Radius)
  {
    this.Radius = Radius;
    this.Center = Center;
    this.Start_Ray = new(Center, A);

    Angle = (A - Center).AngleTo(B - Center);
  }

  public override Point Sample()
  {
    var newAngle = Figure.rnd.RandDoubleRange(0, Angle);

    var vector = this.Start_Ray.DirectorVector.GetRotatedAsVector(newAngle);

    vector = (this.Radius / vector.Norm) * vector;

    return vector + this.Center;
  }

  public override bool Equals(GSObject obj)
  {
    if (obj is Arc A)
    {
      return Functions.EqualVectorsApprox(A.Center, this.Center) &&
          Functions.EqualApprox(this.Radius, A.Radius) &&
          Functions.EqualApprox(this.Angle, A.Angle) &&
          Functions.EqualApprox(A.Start_Ray.DirectorVector.AngleTo(this.Start_Ray.DirectorVector), 0);
    }

    return false;
  }

  public override bool SameTypeAs(GSObject gso) => gso is Arc;

  public override string GetTypeName() => TypeName.Arc.ToString();
  public override string ToString() => $"Arc{{C={Center}, R={this.Start_Ray}, Alpha={this.Angle}, Rad={this.Radius}}}";

}
