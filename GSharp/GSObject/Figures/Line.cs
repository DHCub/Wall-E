namespace GSharp.Objects.Figures;
using System;
using GSharp.Types;
using GSharp.Exceptions;

public partial class Line : GeometricLocation
{

    public readonly Point APoint;

    public readonly Point NormalVector;
    public readonly Point DirectorVector;
    public readonly double AlgebraicTrace;

    public override string ToString()
    {
        return $"L:({NormalVector.XCoord})x + ({NormalVector.YCoord})y + ({AlgebraicTrace}) = 0";
    }

    public Line()
    {
        (Point p1, Point p2) = Point.TwoDifferentPoints();

        this.APoint = p1;
        DirectorVector = p2 - APoint;
        NormalVector = DirectorVector.Orthogonal();
        AlgebraicTrace = -NormalVector.XCoord*APoint.XCoord - NormalVector.YCoord*APoint.YCoord;
    }

    public Line(Point APoint, Point BPoint)
    {
        // if (Functions.EqualVectorsApprox(APoint, BPoint))
        //     throw new RuntimeError(null, "Equal Points Cannot determine a Line");
        
        this.APoint = APoint;
        DirectorVector = BPoint - APoint;
        NormalVector = DirectorVector.Orthogonal();
        AlgebraicTrace = -NormalVector.XCoord*APoint.XCoord - NormalVector.YCoord*APoint.YCoord; 
    }

    public Line(double A, double B, double C)
    {
        var a0 = Functions.EqualApprox(A, 0);
        if (a0 && Functions.EqualApprox(B, 0))
            throw new ArgumentException("Invalid Coefficients");
        
        this.NormalVector = new Point(A, B);
        this.DirectorVector = this.NormalVector.Orthogonal();
        this.AlgebraicTrace = C;
        
        if (a0)
            this.APoint = new Point(0, -C/B);
        else this.APoint = new Point(-C/A, 0);
    }

    public static Line PointDirectorVec(Point Point, Point DirectionVector)
        => new(Point, Point + DirectionVector);

    public override Point Sample()
    {
        var A = this.NormalVector.XCoord;
        var B = this.NormalVector.YCoord;
        var C = this.AlgebraicTrace;

        if(Functions.GreaterThanApprox(Math.Abs(A), Math.Abs(B)))
        {
            var y = Figure.rnd.RandDoubleRange(Figure.WindowStartY, Figure.WindowEndY);

            return new Point(-C/A - B/A*y, y);
        }

        var x = Figure.rnd.RandDoubleRange(Figure.WindowStartX, Figure.WindowEndX); 
        return new Point(x, -C/B - A/B*x);
    }

    public override bool Equals(GSObject obj) => 
        obj is Line L && 
            L.DirectorVector.IsColinear(this.DirectorVector) && 
            Functions.Intersect(this.APoint, L).Count > 0;

    public override string GetTypeName() => TypeName.Line.ToString();

    public override bool SameTypeAs(GSObject gso) => gso is Line;
}