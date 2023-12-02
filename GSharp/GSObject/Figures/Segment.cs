namespace GSharp.GSObject.Figures;
using System;

public partial class Segment : Figure
{
    public Point A_Point { get; }
    public Point B_Point { get; }

    public Segment()
    {
        (Point p1, Point p2) = Point.TwoDifferentPoints();

        this.A_Point = p1;
        this.B_Point = p2;
    }

    public Segment(Point A_Point, Point B_Point)
    {
        if (Functions.Equal_Vectors_Approx(A_Point, B_Point))
            throw new ArgumentException("Equal Points do not Determine a Segment");
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
}