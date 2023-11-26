namespace Geometry;
using System;

public partial class Line : GeoExpr
{

    public Point A_Point {get;}

    public Point Normal_Vector {get;}
    public Point Director_Vector {get;}
    public double Algebraic_Trace {get;}

    public override string ToString()
    {
        return $"({Normal_Vector.X_Coord})x + ({Normal_Vector.Y_Coord})y + ({Algebraic_Trace}) = 0";
    }

    public Line()
    {
        (Point p1, Point p2) = Point.TwoDifferentPoints();

        this.A_Point = p1;
        Director_Vector = p2 - A_Point;
        Normal_Vector = Director_Vector.Orthogonal();
        Algebraic_Trace = -Normal_Vector.X_Coord*A_Point.X_Coord - Normal_Vector.Y_Coord*A_Point.Y_Coord;
    }

    public Line(Point A_Point, Point B_Point)
    {
        if (Functions.Equal_Vectors_Approx(A_Point, B_Point))
            throw new ArgumentException("Equal Points Cannot determine a Line");
        
        this.A_Point = A_Point;
        Director_Vector = B_Point - A_Point;
        Normal_Vector = Director_Vector.Orthogonal();
        Algebraic_Trace = -Normal_Vector.X_Coord*A_Point.X_Coord - Normal_Vector.Y_Coord*A_Point.Y_Coord; 
    }

    public Line(double A, double B, double C)
    {
        var a0 = Functions.Equal_Approx(A, 0);
        if (a0 && Functions.Equal_Approx(B, 0))
            throw new ArgumentException("Invalid Coefficients");
        
        this.Normal_Vector = new Point(A, B);
        this.Director_Vector = this.Normal_Vector.Orthogonal();
        this.Algebraic_Trace = C;
        
        if (a0)
            this.A_Point = new Point(0, -C/B);
        else this.A_Point = new Point(-C/A, 0);
    }

    public static Line Point_DirectorVec(Point Point, Point Direction_Vector)
        => new(Point, Point + Direction_Vector);

    public override Point Sample()
    {
        var A = this.Normal_Vector.X_Coord;
        var B = this.Normal_Vector.Y_Coord;
        var C = this.Algebraic_Trace;

        if(Functions.Greater_Than_Approx(Math.Abs(A), Math.Abs(B)))
        {
            var y = GeoExpr.rnd.RandfRange(IDrawable.Window_StartY, IDrawable.Window_EndY);

            return new Point(-C/A - B/A*y, y);
        }

        var x = GeoExpr.rnd.RandfRange(IDrawable.Window_StartX, IDrawable.Window_EndX); 
        return new Point(x, -C/B - A/B*x);
    }
}