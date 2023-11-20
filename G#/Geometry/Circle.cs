// namespace Geometry;
// using System;


// public partial class Circle : GeoExpr
// {
//     public double Radius {get;}

//     public Point Center {get;}

//     public Circle() 
//     {
//         this.Center = new Point();
        
//         float XMin = Math.Abs((float)Center.X_Coord - Window_StartX);
//         XMin = Math.Min(XMin, Math.Abs((float)Center.X_Coord - Window_EndX));

//         float YMin = Math.Abs((float)Center.Y_Coord - Window_StartY);
//         YMin = Math.Min(YMin, Math.Abs((float)Center.Y_Coord - Window_EndY));

//         float rad = Math.Min(XMin, YMin);
//         rad = Math.Max(rad, 10*Point_Representation_Radius);

//         this.Radius = rad;
//     }

//     public Circle(Point Center, double Radius)
//     {
//         this.Center = Center;
//         this.Radius = Radius;
//     }
// }