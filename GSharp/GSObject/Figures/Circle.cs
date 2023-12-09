namespace GSharp.Objects.Figures;
using System;
using GSharp.Types;

public partial class Circle : GeometricLocation
{
    public readonly double Radius;

    public readonly Point Center;

    public Circle()
    {
        this.Center = new Point();

        double XMin = Math.Abs(Center.XCoord - Figure.WindowStartX);
        XMin = Math.Min(XMin, Math.Abs(Center.XCoord - Figure.WindowEndX));

        double YMin = Math.Abs(Center.YCoord - Figure.WindowStartY);
        YMin = Math.Min(YMin, Math.Abs(Center.YCoord - Figure.WindowEndY));

        double rad = Math.Min(XMin, YMin);
        rad = Math.Max(rad, 10 * Figure.PointRepresentationRadius / Figure.ZoomFactor);

        this.Radius = rad;
    }

    public Circle(Point Center, double Radius)
    {
        this.Center = Center;
        this.Radius = Radius;
    }

    public override Point Sample()
    {
        var angle = Figure.rnd.RandDoubleRange(0, 2 * Math.PI);

        var vector = new Point(200, 0).GetRotatedAsVector(angle);

        vector = (this.Radius / vector.Norm) * vector;

        var p = Center + vector;

        return p;
    }

    public override bool Equals(GSObject obj) 
        => obj is Circle C && Functions.EqualVectorsApprox(C.Center, this.Center) && Functions.EqualApprox(this.Radius, C.Radius);



    public override string GetTypeName() => TypeName.Circle.ToString();
    public override string ToString() => $"C({Center})";

    public override bool SameTypeAs(GSObject gso) => gso is Circle;
    public override bool SameTypeAs(GSType gst) => gst.SameTypeAs(TypeName.Circle);
}