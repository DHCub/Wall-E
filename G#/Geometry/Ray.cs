// namespace Geometry;
// using System;

// public partial class Ray : GeoExpr
// {
//     public Point First_Point {get;}

//     public Point Director_Vector {get;}

//     public Ray() : this(new Point(), new Point()) {}

//     public Ray(Point First_Point, Point Second_Point)
//     {
//         if (Functions.Equal_Vectors_Approx(First_Point, Second_Point))
//             throw new ArgumentException("Equal Points Cannot determine a Ray");
        
//         this.First_Point = First_Point;
//         this.Director_Vector = Second_Point - First_Point;
//     }
// }