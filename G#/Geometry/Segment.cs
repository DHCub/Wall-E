namespace Geometry;
using System;

public partial class Segment : GeoExpr
{
    public Point A_Point {get;}
    public Point B_Point {get;}

    public Segment() : this(new Point(), new Point()) {}

    public Segment(Point A_Point, Point B_Point)
    {
        if (Functions.Equal_Vectors_Approx(A_Point, B_Point))
            throw new ArgumentException("Equal Points do not Determine a Segment");
        this.A_Point = A_Point;
        this.B_Point = B_Point;
    }
}