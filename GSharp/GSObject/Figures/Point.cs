namespace GSharp.Objects.Figures;
using System;
using GSharp.Types;
using GSharp.Exceptions;
public class Point : Figure
{
    public readonly double X_Coord;
    public readonly double Y_Coord;
    public double Norm
    {
        get
        {
            return Math.Sqrt(this.X_Coord * this.X_Coord + this.Y_Coord * this.Y_Coord);
        }
    }


    public override string ToString()
    {
        return $"({X_Coord}, {Y_Coord})";
    }

    public Point() : this(
        Figure.rnd.RandfRange(Figure.Window_StartX, Figure.Window_EndX),
        Figure.rnd.RandfRange(Figure.Window_StartY, Figure.Window_EndY))
    { }

    public Point(double X_Coord, double Y_Coord)
    {
        this.X_Coord = X_Coord;
        this.Y_Coord = Y_Coord;
    }

    public static Point operator -(Point A, Point B)
        => new(A.X_Coord - B.X_Coord, A.Y_Coord - B.Y_Coord);

    public static Point operator +(Point A, Point B)
        => new(A.X_Coord + B.X_Coord, A.Y_Coord + B.Y_Coord);

    public static Point operator *(double alpha, Point A)
        => new(A.X_Coord * alpha, A.Y_Coord * alpha);

    public static Point operator *(Point A, double alpha)
        => new(A.X_Coord * alpha, A.Y_Coord * alpha);
    
    public static Point operator / (Point P, double alpha)
        => new(P.X_Coord/alpha, P.Y_Coord/alpha);

    public double Dot_Product(Point other) => this.X_Coord * other.X_Coord + this.Y_Coord * other.Y_Coord;

    public bool IsColinear(Point other)
    {
        if (this.isOrigin() || other.isOrigin()) return true;
        var cos = this.Dot_Product(other);
        var norm_Mult = this.Norm * other.Norm;

        return Functions.Equal_Approx(cos, -norm_Mult) || Functions.Equal_Approx(cos, norm_Mult);
    }

    public bool isOrigin()
        => Functions.Equal_Approx(this.X_Coord, 0) && Functions.Equal_Approx(this.Y_Coord, 0);

    public double AngleTo(Point other)
    {
        var cos_times_Norm = this.Dot_Product(other);
        var sin_times_Norm = this.X_Coord * other.Y_Coord - this.Y_Coord * other.X_Coord;

        var angle = Math.Atan2(sin_times_Norm, cos_times_Norm);
        if (angle < 0) angle = angle + 2 * Math.PI;

        return angle;
    }

    public Point Orthogonal()
        => new(this.Y_Coord, -this.X_Coord);

    public bool Equal_Approx(Point other)
        => Functions.Equal_Vectors_Approx(this, other);

    public double Distance_To(Point other)
        => Functions.Distance(this, other);

    public double Distance_To(Line L)
        => Functions.Distance(this, L);

    public Point GetRotatedAsVector(double Angle)
    {
        var x2 = Math.Cos(Angle) * X_Coord - Math.Sin(Angle) * Y_Coord;
        var y2 = Math.Sin(Angle) * X_Coord + Math.Cos(Angle) * Y_Coord;

        return new(x2, y2);
    }

    public override Point Sample() => this;

    public static (Point p1, Point p2) TwoDifferentPoints()
    {
        Point p1 = new();
        Point p2 = new();

        if (!Functions.Equal_Vectors_Approx(p1, p2)) return (p1, p2);

        p2 = new Point(1E-7, 0);
        p2 = p2.GetRotatedAsVector(Figure.rnd.RandfRange(0, (float)(2 * Math.PI)));

        p2 = p1 + p2;

        return (p1, p2);
    }

    public override string GetTypeName() => TypeName.Point.ToString();

    public override bool Equals(GSObject obj) => obj is Point P && Functions.Equal_Vectors_Approx(this, P);

    public override bool SameTypeAs(GSObject gso) => gso is Point;

    public override GSObject OperateScalar(Scalar other, Mult op)
        => this*other.value;
    public override GSObject OperateScalar(Scalar other, Div op)
        => this/other.value;
   


    public override GSObject OperateMeasure(Measure other, Div op)
        => this/other.value;
    public override GSObject OperateMeasure(Measure other, Mult op)
        => this*other.value;

    public override GSObject OperatePoint(Point other, Add op)
        => this + other;

    public override GSObject OperatePoint(Point other, Subst op)
        => this - other;


}