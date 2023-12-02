namespace GSharp.Objects.Figures;
using System;
using GSharp.Types;

public partial class Ray : GeometricLocation
{
    public readonly Point First_Point;

    public readonly Point Director_Vector;

    public Ray()
    {
        (Point p1, Point p2) = Point.TwoDifferentPoints();

        this.First_Point = p1;
        this.Director_Vector = p2 - First_Point;
    }

    public Ray(Point First_Point, Point Second_Point)
    {
        if (Functions.Equal_Vectors_Approx(First_Point, Second_Point))
            throw new RuntimeError("Equal Points Cannot determine a Ray");

        this.First_Point = First_Point;
        this.Director_Vector = Second_Point - First_Point;
    }

    public static Ray Point_DirectorVec(Point Point, Point Direction_Vector)
        => new(Point, Point + Direction_Vector);

    public override Point Sample()
    {
        throw new NotImplementedException();
    }
    public override string ToString() => $"Ray: [from: {this.First_Point} directorV: {this.Director_Vector}]";

    public override bool Equals(GSObject obj)
        => obj is Ray R &&
            Functions.Equal_Vectors_Approx(R.First_Point, this.First_Point) &&
            Functions.Equal_Approx(0, this.Director_Vector.AngleTo(R.Director_Vector));

    public override string GetTypeName() => TypeName.Ray.ToString();
}