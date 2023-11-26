namespace Geometry;
using System;

public partial class Ray : GeoExpr
{
    public Point First_Point {get;}

    public Point Director_Vector {get;}

    public Ray()
    {
        (Point p1, Point p2) = Point.TwoDifferentPoints();

        this.First_Point = p1;
        this.Director_Vector = p2 - First_Point;
    }   

    public Ray(Point First_Point, Point Second_Point)
    {
        if (Functions.Equal_Vectors_Approx(First_Point, Second_Point))
            throw new ArgumentException("Equal Points Cannot determine a Ray");
        
        this.First_Point = First_Point;
        this.Director_Vector = Second_Point - First_Point;
    }

    public static Ray Point_DirectorVec(Point Point, Point Direction_Vector)
        => new(Point, Point + Direction_Vector);

    public override Point Sample()
    {
        throw new NotImplementedException();
    }
}