namespace GSharp.GSObject.Figures;

using System;
using GSharp.GSObject.Collections;
using GSharp.Statement;

// using Godot;

public class Arc : Figure
{
    public Ray Start_Ray { get; }

    public Point Center { get; }

    public double Radius { get; }
    public double Angle { get; }

    public Arc()
    {
        var core = new Circle();

        this.Start_Ray = new Ray(core.Center, new Point());
        this.Angle = Figure.rnd.RandfRange(0, (float)(2 * Math.PI));
        this.Center = core.Center;
        this.Radius = core.Radius;
    }

    public Arc(Ray Start_Ray, Ray End_Ray, double Radius)
    {
        this.Start_Ray = Start_Ray;
        this.Center = Start_Ray.First_Point;
        this.Radius = Radius;

        Angle = Start_Ray.Director_Vector.AngleTo(End_Ray.Director_Vector);
    }

    public override Point Sample()
    {
        var newAngle = Figure.rnd.RandfRange(0, (float)Angle);

        var vector = this.Start_Ray.Director_Vector.GetRotatedAsVector(newAngle);

        vector = (this.Radius / vector.Norm) * vector;

        return vector + this.Center;
    }

    public override bool Equals(GSObject obj)
        {
        if (obj is Arc A)
        {
            return Functions.Equal_Vectors_Approx(A.Center, this.Center) &&
                Functions.Equal_Approx(this.Radius, A.Radius) &&
                Functions.Equal_Approx(this.Angle, A.Angle) &&
                Functions.Equal_Approx(A.Start_Ray.Director_Vector.AngleTo(this.Start_Ray.Director_Vector), 0);
        }

        return false;
        }

    public override string GetTypeName() => GSTypes.Arc.ToString();
    public override string ToString() => $"Arc{{C={Center}, R={this.Start_Ray}, Alpha={this.Angle}, Rad={this.Radius}}}";

    public override GSObject OperatePoint(Point other, Add op) => UnsupportedOperation(other, op);
    public override GSObject OperatePoint(Point other, Subst op) => UnsupportedOperation(other, op);

    
    public override GSObject OperateScalar(Scalar other, Mult op)  => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, Div op)  => UnsupportedOperation(other, op);




}
