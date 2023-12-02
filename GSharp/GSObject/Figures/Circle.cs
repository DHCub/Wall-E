namespace GSharp.Objects.Figures;
using System;
using Godot;

public partial class Circle : GeometricLocation
{
    public readonly double Radius;

    public readonly Point Center;

    public Circle()
    {
        this.Center = new Point();

        float XMin = Math.Abs((float)Center.X_Coord - Figure.Window_StartX);
        XMin = Math.Min(XMin, Math.Abs((float)Center.X_Coord - Figure.Window_EndX));

        float YMin = Math.Abs((float)Center.Y_Coord - Figure.Window_StartY);
        YMin = Math.Min(YMin, Math.Abs((float)Center.Y_Coord - Figure.Window_EndY));

        float rad = Math.Min(XMin, YMin);
        rad = Math.Max(rad, 10 * Figure.Point_Representation_Radius / Figure.ZoomFactor);

        this.Radius = rad;
    }

    public Circle(Point Center, double Radius)
    {
        this.Center = Center;
        this.Radius = Radius;
    }

    public override Point Sample()
    {
        var angle = Figure.rnd.RandfRange(0, (float)(2 * Math.PI));

        var vector = new Point(200, 0).GetRotatedAsVector(angle);

        vector = (this.Radius / vector.Norm) * vector;

        var p = Center + vector;

        return p;
    }

    public override bool Equals(GSObject obj) 
        => obj is Circle C && Functions.Equal_Vectors_Approx(C.Center, this.Center) && Functions.Equal_Approx(this.Radius, C.Radius);



    public override string GetTypeName() => TypeName.Circle.ToString();
    public override string ToString() => $"C({Center})";

}