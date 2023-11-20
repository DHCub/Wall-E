// namespace Geometry;
// using GSharp;
// using System;
// using System.Collections.Generic;

// static class Functions
// {
//     public static double Distance(Point Point_A, Point Point_B)
//         => (Point_B - Point_A).Norm;

//     public static double Distance(Point Point, Line L)
//         => Math.Abs(L.Normal_Vector.X_Coord * Point.X_Coord + L.Normal_Vector.Y_Coord * Point.X_Coord + L.Algebraic_Trace) /
//            L.Normal_Vector.Norm;

//     #region Point

//     public static Finite_Static_Seqence<Point> Intersect(Point Point, Line L) => Intersect(L, Point);
//     public static Finite_Static_Seqence<Point> Intersect(Point Point, Ray R) => Intersect(R, Point);

//     public static Finite_Static_Seqence<Point> Intersect(Point Point, Circle C)
//     {
//         if (Equal_Approx(Distance(Point, C.Center), C.Radius))
//             return new Finite_Static_Seqence<Point>(new Point[] { Point });

//         return new Finite_Static_Seqence<Point>();
//     }

//     public static Finite_Static_Seqence<Point> Intersect(Point Point, Arc A)
//     {
//         var list = new List<Point>();

//         if (Equal_Approx(Distance(Point, A.Center), A.Radius))
//         {
//             var CP_Vector = Point - A.Center;
//             if (Less_Equal_Approx(A.Start_Ray.Director_Vector.AngleTo(CP_Vector), A.Angle)) list.Add(Point);
//         }

//         return new Finite_Static_Seqence<Point>(list);
//     }

//     #endregion

//     #region Line

//     public static Finite_Static_Seqence<Point> Intersect(Line L, Point Point)
//     {
//         if ((L.A_Point - Point).isColinear(L.Direction_Vector))
//             return new Finite_Static_Seqence<Point>(new List<Point>(new Point[] { Point }));

//         return new Finite_Static_Seqence<Point>();
//     }


//     public static Sequence<Point> Intersect(Line A, Line B)
//     {
//         double A1 = A.Normal_Vector.X_Coord;
//         double B1 = A.Normal_Vector.Y_Coord;
//         double C1 = -A.Algebraic_Trace;

//         double A2 = B.Normal_Vector.X_Coord;
//         double B2 = B.Normal_Vector.Y_Coord;
//         double C2 = -B.Algebraic_Trace;

//         double Determinant = A1 * B2 - B1 * A2;
//         // paralell lines
//         if (Equal_Approx(Determinant, 0))
//         {
//             // same lines
//             if (Equal_Approx(C1, C2))
//                 return new Infinite_Static_Sequence<Point>();

//             // no intersection
//             return new Finite_Static_Seqence<Point>();
//         }

//         var intersection = new Point((C1 * B2 - B1 * C2) / Determinant, (A1 * C2 - C1 * A2) / Determinant);
//         var list = new List<Point>();
//         list.Add(intersection);

//         return new Finite_Static_Seqence<Point>(list);
//     }

//     public static Sequence<Point> Intersect(Line A, Ray R)
//     {
//         Line R_Line = Line.Point_DirectorVec(R.First_Point, R.Director_Vector);
//         var Line_intersect = Intersect(A, R_Line);

//         if (Line_intersect is Finite_Static_Seqence<Point>)
//         {
//             var intersection = (Finite_Static_Seqence<Point>)Line_intersect;

//             if (intersection.Count == 0) return intersection;

//             var P = intersection[0];

//             return Intersect(P, R);
//         }

//         return Line_intersect;
//     }


//     public static Finite_Static_Seqence<Point> Intersect(Line L, Circle C)
//     {

//         if (Equal_Approx(C.Radius, 0))
//         {
//             return Intersect(L, C.Center);
//         }

//         // check if vector isn't null before
//         Point scale_Vector(Point A, double length)
//         {
//             if (A.Norm == 0) return A;
//             return (length / A.Norm) * A;
//         }


//         // we move the circle to the origin, we move the line with it
//         var L_Prime = Line.Point_DirectorVec(L.A_Point - C.Center, L.Direction_Vector);

//         var origin = new Point(0, 0);
//         // here we are taking the point in L which is closest to the origin
//         var Line_Point = Intersect(Line.Point_DirectorVec(origin, L_Prime.Normal_Vector), L_Prime)[0]!;
//         // this will not be null because perpendicular lines always intersect


//         var list = new List<Point>();

//         if (!Equal_Vectors_Approx(Line_Point, origin))
//         {
//             var d = Distance(Line_Point, origin);
//             if (Equal_Approx(d, C.Radius))
//             {
//                 // this would mean the line is tangent to the centered circumference
//                 list.Add(Line_Point + C.Center);
//                 return new Finite_Static_Seqence<Point>(list);
//             }
//             if (Greater_Than_Approx(d, C.Radius))
//                 return new Finite_Static_Seqence<Point>();


//             var Point_To_Intersection_D = Math.Sqrt(C.Radius * C.Radius - d * d);

//             var scaled_Direction = scale_Vector(L.Direction_Vector, Point_To_Intersection_D);

//             list.Add(Line_Point + scaled_Direction + C.Center);
//             list.Add(Line_Point - scaled_Direction + C.Center);

//             return new Finite_Static_Seqence<Point>(list);
//         }


//         var radius_Vector = scale_Vector(L.Direction_Vector, C.Radius);

//         list.Add(origin + radius_Vector + C.Center);
//         list.Add(origin - radius_Vector + C.Center);
//         return new Finite_Static_Seqence<Point>(list);
//     }


//     public static Finite_Static_Seqence<Point> Intersect(Line L, Arc A)
//     {
//         var circle = new Circle(A.Center, A.Radius);

//         var intersection = Intersect(L, circle);

//         if (intersection.Count == 0) return intersection;

//         var list = new List<Point>();

//         foreach (var P in intersection)
//         {
//             var CP_Vector = P - A.Center;
//             if (Less_Equal_Approx(A.Start_Ray.Director_Vector.AngleTo(CP_Vector), A.Angle)) list.Add(P);
//         }

//         return new Finite_Static_Seqence<Point>(list);
//     }

//     #endregion

//     #region Ray

//     public static Finite_Static_Seqence<Point> Intersect(Ray R, Point Point)
//     {
//         var AP_vector = Point - R.First_Point;

//         if (!AP_vector.isColinear(R.Director_Vector)) return new Finite_Static_Seqence<Point>();

//         if (AP_vector.Dot_Product(R.Director_Vector) < 0) return new Finite_Static_Seqence<Point>();
//         else return new Finite_Static_Seqence<Point>(new Point[] { Point });
//     }


//     public static Sequence<Point> Intersect(Ray R, Line L) => Intersect(L, R);

//     public static Sequence<Point> Intersect(Ray A, Ray B)
//     {
//         var Line_A = Line.Point_DirectorVec(A.First_Point, A.Director_Vector);
//         var Line_B = Line.Point_DirectorVec(B.First_Point, B.Director_Vector);

//         var Line_intersect = Functions.Intersect(Line_A, Line_B);


//         if (Line_intersect is Finite_Static_Seqence<Point>)
//         {
//             var list = new List<Point>();
//             var intersection = (Finite_Static_Seqence<Point>)Line_intersect;

//             if (intersection.Count == 0) return intersection;

//             intersection = Intersect(intersection[0], A);
//             if (intersection.Count == 0) return intersection;

//             intersection = Intersect(intersection[0], B);
//             return intersection;
//         }

//         return Line_intersect;
//     }

//     public static Finite_Static_Seqence<Point> Intersect(Ray R, Circle C)
//     {
//         var Line_R = Line.Point_DirectorVec(R.First_Point, R.Director_Vector);

//         var intersection = Intersect(Line_R, C);

//         var list = new List<Point>(2);

//         foreach (var P in intersection)
//         {
//             if (Intersect(P, R).Count > 0) list.Add(P);
//         }

//         return new Finite_Static_Seqence<Point>(list);
//     }


//     public static Finite_Static_Seqence<Point> Intersect(Ray R, Arc A)
//     {
//         var C = new Circle(A.Center, A.Radius);
//         var intersection = Intersect(R, C);

//         var list = new List<Point>(2);

//         foreach (var P in intersection)
//         {
//             if (Intersect(P, A).Count > 0) list.Add(P);
//         }

//         return new Finite_Static_Seqence<Point>(list);
//     }

//     #endregion

//     #region Circle

//     public static Finite_Static_Seqence<Point> Intersect(Circle C, Point Point)
//         => Intersect(Point, C);


//     public static Finite_Static_Seqence<Point> Intersect(Circle C, Line L) => Intersect(L, C);

//     public static Finite_Static_Seqence<Point> Intersect(Circle C, Ray R) => Intersect(R, C);

//     public static Sequence<Point> Intersect(Circle C1, Circle C2)
//     {
//         if (Equal_Vectors_Approx(C1.Center, C2.Center))
//         {
//             if (Equal_Approx(C1.Radius, C2.Radius))
//                 return new Infinite_Static_Sequence<Point>();

//             return new Finite_Static_Seqence<Point>();
//         }

//         var k1 = C1.Center.X_Coord;
//         var h1 = C1.Center.Y_Coord;
//         var r1 = C1.Radius;

//         var k2 = C2.Center.X_Coord;
//         var h2 = C2.Center.Y_Coord;
//         var r2 = C2.Radius;

//         var A = 2 * (k2 - k1);
//         var B = 2 * (h2 - h1);
//         var C = -(r1 * r1 - r2 * r2 - k1 * k1 + k2 * k2 - h1 * h1 + h2 * h2);

//         var Line = new Line(A, B, C);


//         return Intersect(Line, C1);
//     }

//     public static Sequence<Point> Intersect(Circle C, Arc A)
//     {
//         var intersections = Intersect(C, new Circle(A.Center, A.Radius));

//         if (intersections is Finite_Static_Seqence<Point>)
//         {
//             var list = new List<Point>();
//             foreach (var P in (Finite_Static_Seqence<Point>)intersections)
//             {
//                 if (Intersect(P, A).Count > 0) list.Add(P);
//             }

//             return new Finite_Static_Seqence<Point>(list);
//         }

//         return intersections;
//     }

//     #endregion

//     #region Arc

//     public static Finite_Static_Seqence<Point> Intersect(Arc A, Point Point) => Intersect(Point, A);

//     public static Finite_Static_Seqence<Point> Intersect(Arc A, Line L) => Intersect(L, A);

//     public static Finite_Static_Seqence<Point> Intersect(Arc A, Ray R) => Intersect(R, A);

//     public static Sequence<Point> Intersect(Arc A, Circle C)
//         => Intersect(C, A);

//     public static Sequence<Point> Intersect(Arc A1, Arc A2)
//     {
//         var intersections = Intersect(new Circle(A1.Center, A1.Radius), A2);

//         if (intersections is Finite_Static_Seqence<Point>)
//         {
//             var list = new List<Point>();
//             foreach (var P in (Finite_Static_Seqence<Point>)intersections)
//             {
//                 if (Intersect(P, A1).Count > 0) list.Add(P);
//             }

//             return new Finite_Static_Seqence<Point>(list);
//         }


//         return intersections;
//     }

//     #endregion

//     #region Comparers

//     public static int Approx_Compare_Double(double A, double B)
//     {
//         const double Epsilon = 1E-12;
//         if (Math.Abs(A - B) <= Epsilon) return 0;
//         if (A < B) return -1;
//         // (A > B) 
//         return 1;
//     }

//     public static bool Greater_Than_Approx(double A, double B) => Approx_Compare_Double(A, B) == 1;
//     public static bool Less_Than_Approx(double A, double B) => Approx_Compare_Double(A, B) == -1;
//     public static bool Equal_Approx(double A, double B) => Approx_Compare_Double(A, B) == 0;
//     public static bool Equal_Vectors_Approx(Point A, Point B)
//         => Equal_Approx(A.X_Coord, B.X_Coord) && Equal_Approx(A.Y_Coord, B.Y_Coord);
//     public static bool Greater_Equal_Approx(double A, double B) => Approx_Compare_Double(A, B) >= 0;
//     public static bool Less_Equal_Approx(double A, double B) => Approx_Compare_Double(A, B) <= 0;


//     #endregion
// }