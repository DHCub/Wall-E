// namespace Geometry;

// using System;
// // using Godot;

// public partial class Arc : GeoExpr
// {
//     public Ray Start_Ray {get;}

//     public Point Center {get;}

//     public double Radius {get;}
//     public double Angle {get;}
 
//     public Arc()
//     {
//         var core = new Circle();
        
//         this.Start_Ray = new Ray(core.Center, new Point());
//         this.Angle = GeoExpr.rnd.RandfRange(0, (float)(2*Math.PI));
//         this.Center = core.Center;
//         this.Radius = core.Radius;
//     }

//     public Arc(Ray Start_Ray, Ray End_Ray, double Radius)
//     {
//         this.Start_Ray = Start_Ray;
//         this.Center = Start_Ray.First_Point;
//         this.Radius = Radius;

//         Angle = Start_Ray.Director_Vector.AngleTo(End_Ray.Director_Vector);
//     }
// }
