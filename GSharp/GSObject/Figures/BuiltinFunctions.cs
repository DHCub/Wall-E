namespace GSharp;

using Objects.Figures;

using GSharp.Objects.Collections;
using System;
using System.Collections.Generic;

static class Functions
{
  public static double Distance(Point Point_A, Point Point_B)
      => (Point_B - Point_A).Norm;

  public static double Distance(Point Point, Line L)
      => Math.Abs(L.Normal_Vector.X_Coord * Point.X_Coord + L.Normal_Vector.Y_Coord * Point.X_Coord + L.Algebraic_Trace) /
         L.Normal_Vector.Norm;

  public static double Distance(Line L, Point P)
      => Distance(P, L);

  public static Sequence Intersect(Figure A, Figure B)
  {
    if (A is Point P)
    {
      if (B is Point P2)
        if (Equal_Vectors_Approx(P, P2))
          return new FiniteStaticSequence(new Point[] { P });
        else return new FiniteStaticSequence();

      if (B is Line L1)
        return Intersect(P, L1);

      if (B is Ray R1)
        return Intersect(P, R1);

      if (B is Segment S1)
        return Intersect(P, S1);

      if (B is Circle C1)
        return Intersect(P, C1);

      if (B is Arc Arc1)
        return Intersect(P, Arc1);
    }

    else if (B is Point)
      return Intersect(B, A);

    else if (A is Line L)
    {
      if (B is Line L1)
        return Intersect(L, L1);

      if (B is Ray R1)
        return Intersect(L, R1);

      if (B is Segment S1)
        return Intersect(L, S1);

      if (B is Circle C1)
        return Intersect(L, C1);

      if (B is Arc Arc1)
        return Intersect(L, Arc1);
    }

    else if (B is Line)
      return Intersect(B, A);

    else if (A is Ray R)
    {
      if (B is Ray R1)
        return Intersect(R, R1);

      if (B is Segment S1)
        return Intersect(R, S1);

      if (B is Circle C1)
        return Intersect(R, C1);

      if (B is Arc Arc1)
        return Intersect(R, Arc1);
    }

    else if (B is Ray)
      return Intersect(B, A);

    else if (A is Segment S)
    {
      if (B is Segment S1)
        return Intersect(S, S1);

      if (B is Circle C1)
        return Intersect(S, C1);

      if (B is Arc Arc1)
        return Intersect(S, Arc1);
    }

    else if (B is Segment)
      return Intersect(B, A);

    else if (A is Circle C)
    {
      if (B is Circle C1)
        return Intersect(C, C1);

      else if (B is Arc Arc1)
        return Intersect(C, Arc1);
    }

    else if (B is Circle)
      return Intersect(B, A);

    else if (A is Arc Arc && B is Arc Arc2)
      return Intersect(Arc, Arc2);

    throw new Exception("UNRECOGNIZED IDrawable");

  }


  #region Point

  public static FiniteStaticSequence Intersect(Point Point, Line L) => Intersect(L, Point);
  public static FiniteStaticSequence Intersect(Point Point, Ray R) => Intersect(R, Point);

  public static FiniteStaticSequence Intersect(Point Point, Segment S)
  {
    Ray R1 = new(S.A_Point, S.B_Point);
    Ray R2 = new(S.B_Point, S.A_Point);

    var intersect = Intersect(Point, R1);
    if (intersect.Count == 0) return new();

    return Intersect((Point)intersect[0], R2);
  }

  public static FiniteStaticSequence Intersect(Point Point, Circle C)
  {
    if (Equal_Approx(Distance(Point, C.Center), C.Radius))
      return new FiniteStaticSequence(new Point[] { Point });

    return new FiniteStaticSequence();
  }

  public static FiniteStaticSequence Intersect(Point Point, Arc A)
  {
    var list = new List<Objects.GSObject>();

    if (Equal_Approx(Distance(Point, A.Center), A.Radius))
    {
      var CP_Vector = Point - A.Center;
      if (Less_Equal_Approx(A.Start_Ray.Director_Vector.AngleTo(CP_Vector), A.Angle)) list.Add(Point);
    }

    return new FiniteStaticSequence(list);
  }

  #endregion

  #region Line

  public static FiniteStaticSequence Intersect(Line L, Point Point)
  {
    if ((L.A_Point - Point).IsColinear(L.Director_Vector))
      return new FiniteStaticSequence(new List<Objects.GSObject>(new Point[] { Point }));

    return new FiniteStaticSequence();
  }


  public static Sequence Intersect(Line A, Line B)
  {
    double A1 = A.Normal_Vector.X_Coord;
    double B1 = A.Normal_Vector.Y_Coord;
    double C1 = -A.Algebraic_Trace;

    double A2 = B.Normal_Vector.X_Coord;
    double B2 = B.Normal_Vector.Y_Coord;
    double C2 = -B.Algebraic_Trace;

    double Determinant = A1 * B2 - B1 * A2;
    // paralell lines
    if (Equal_Approx(Determinant, 0))
    {
      // same lines
      if (Equal_Approx(C1, C2))
        return new InfiniteStaticSequence();

      // no intersection
      return new FiniteStaticSequence();
    }

    var intersection = new Point((C1 * B2 - B1 * C2) / Determinant, (A1 * C2 - C1 * A2) / Determinant);
    var list = new List<Objects.GSObject>();
    list.Add(intersection);

    return new FiniteStaticSequence(list);
  }

  public static Sequence Intersect(Line A, Ray R)
  {
    Line R_Line = Line.Point_DirectorVec(R.First_Point, R.Director_Vector);
    var Line_intersect = Intersect(A, R_Line);

    if (Line_intersect is FiniteStaticSequence)
    {
      var intersection = (FiniteStaticSequence)Line_intersect;

      if (intersection.Count == 0) return intersection;

      var P = intersection[0];

      return Intersect((Point)P, R);
    }

    return Line_intersect;
  }

  public static Sequence Intersect(Line A, Segment S)
  {
    Line S_Line = new(S.A_Point, S.B_Point);

    var line_intersection = Intersect(A, S_Line);

    if (line_intersection is FiniteStaticSequence intersect)
    {
      // parallel lines
      if (intersect.Count == 0) return intersect;

      // can only intersect in one point
      return Intersect((Point)intersect[0], S);
    }
    return line_intersection;

  }
  public static FiniteStaticSequence Intersect(Line L, Circle C)
  {

    if (Equal_Approx(C.Radius, 0))
    {
      return Intersect(L, C.Center);
    }

    // check if vector isn't null before
    Point scale_Vector(Point A, double length)
    {
      if (A.Norm == 0) return A;
      return (length / A.Norm) * A;
    }


    // we move the circle to the origin, we move the line with it
    var L_Prime = Line.Point_DirectorVec(L.A_Point - C.Center, L.Director_Vector);

    var origin = new Point(0, 0);
    // here we are taking the point in L which is closest to the origin
    var Line_Point = (Point)Intersect(Line.Point_DirectorVec(origin, L_Prime.Normal_Vector), L_Prime)[0]!;
    // this will not be null because perpendicular lines always intersect


    var list = new List<Objects.GSObject>();

    if (!Equal_Vectors_Approx(Line_Point, origin))
    {
      var d = Distance(Line_Point, origin);
      if (Equal_Approx(d, C.Radius))
      {
        // this would mean the line is tangent to the centered circumference
        list.Add(Line_Point + C.Center);
        return new FiniteStaticSequence(list);
      }
      if (Greater_Than_Approx(d, C.Radius))
        return new FiniteStaticSequence();


      var Point_To_Intersection_D = Math.Sqrt(C.Radius * C.Radius - d * d);

      var scaled_Direction = scale_Vector(L.Director_Vector, Point_To_Intersection_D);

      list.Add(Line_Point + scaled_Direction + C.Center);
      list.Add(Line_Point - scaled_Direction + C.Center);

      return new FiniteStaticSequence(list);
    }


    var radius_Vector = scale_Vector(L.Director_Vector, C.Radius);

    list.Add(origin + radius_Vector + C.Center);
    list.Add(origin - radius_Vector + C.Center);
    return new FiniteStaticSequence(list);
  }


  public static FiniteStaticSequence Intersect(Line L, Arc A)
  {
    var circle = new Circle(A.Center, A.Radius);

    var intersection = Intersect(L, circle);

    if (intersection.Count == 0) return intersection;

    var list = new List<Objects.GSObject>();

    foreach (var P in intersection)
    {
      var CP_Vector = (Point)P - A.Center;
      if (Less_Equal_Approx(A.Start_Ray.Director_Vector.AngleTo(CP_Vector), A.Angle)) list.Add(P);
    }

    return new FiniteStaticSequence(list);
  }

  #endregion

  #region Ray

  public static FiniteStaticSequence Intersect(Ray R, Point Point)
  {
    var AP_vector = Point - R.First_Point;

    if (!AP_vector.IsColinear(R.Director_Vector)) return new FiniteStaticSequence();

    if (Less_Than_Approx(AP_vector.Dot_Product(R.Director_Vector), 0)) return new FiniteStaticSequence();
    else return new FiniteStaticSequence(new Point[] { Point });
  }


  public static Sequence Intersect(Ray R, Line L) => Intersect(L, R);

  public static Sequence Intersect(Ray R, Ray B)
  {
    var Line_A = Line.Point_DirectorVec(R.First_Point, R.Director_Vector);
    var Line_B = Line.Point_DirectorVec(B.First_Point, B.Director_Vector);

    var Line_intersect = Functions.Intersect(Line_A, Line_B);


    if (Line_intersect is FiniteStaticSequence)
    {
      var list = new List<Objects.GSObject>();
      var intersection = (FiniteStaticSequence)Line_intersect;

      if (intersection.Count == 0) return intersection;

      intersection = Intersect((Point)intersection[0], R);
      if (intersection.Count == 0) return intersection;

      intersection = Intersect((Point)intersection[0], B);
      return intersection;
    }

    return Line_intersect;
  }

  public static Sequence Intersect(Ray R, Segment S)
  {
    var line_intersect = Intersect(new Line(R.First_Point, R.First_Point + R.Director_Vector), S);

    if (line_intersect is FiniteStaticSequence intersect)
    {
      // parallel
      if (intersect.Count == 0) return line_intersect;
      // can only intersect in one point
      return Intersect((Point)intersect[0], R);
    }

    return line_intersect;
  }

  public static FiniteStaticSequence Intersect(Ray R, Circle C)
  {
    var Line_R = Line.Point_DirectorVec(R.First_Point, R.Director_Vector);

    var intersection = Intersect(Line_R, C);


    var list = new List<Objects.GSObject>(2);

    foreach (var P in intersection)
    {
      if (Intersect((Point)P, R).Count > 0) list.Add(P);

    }

    return new FiniteStaticSequence(list);
  }


  public static FiniteStaticSequence Intersect(Ray R, Arc A)
  {
    var C = new Circle(A.Center, A.Radius);
    var intersection = Intersect(R, C);

    var list = new List<Objects.GSObject>(2);

    foreach (var P in intersection)
    {
      if (Intersect((Point)P, A).Count > 0) list.Add(P);
    }

    return new FiniteStaticSequence(list);
  }

  #endregion

  #region Segment

  public static FiniteStaticSequence Intersect(Segment S, Point Point)
      => Intersect(Point, S);

  public static Sequence Intersect(Segment S, Line L)
      => Intersect(L, S);

  public static Sequence Intersect(Segment S, Ray R)
      => Intersect(R, S);

  public static Sequence Intersect(Segment S1, Segment S2)
  {
    Ray R1 = new(S1.A_Point, S1.B_Point);
    Ray R2 = new(S1.B_Point, S1.A_Point);

    var ray_intersect = Intersect(R1, S2);
    if (ray_intersect is FiniteStaticSequence intersect)
    {
      if (intersect.Count == 0) return ray_intersect;
      return Intersect((Point)intersect[0], R2);
    }

    return ray_intersect;
  }

  public static FiniteStaticSequence Intersect(Segment S, Circle C)
  {
    List<Objects.GSObject> list = new(2);

    foreach (var P in Intersect(new Line(S.A_Point, S.B_Point), C))
    {
      if (Intersect((Point)P, S).Count > 0) list.Add(P);
    }

    return new(list);
  }

  public static FiniteStaticSequence Intersect(Segment S, Arc A)
  {
    Circle C = new(A.Center, A.Radius);

    List<Objects.GSObject> list = new(2);

    foreach (var P in Intersect(S, C))
    {
      if (Intersect((Point)P, A).Count > 0)
        list.Add(P);
    }

    return new(list);
  }

  #endregion

  #region Circle

  public static FiniteStaticSequence Intersect(Circle C, Point Point)
      => Intersect(Point, C);


  public static FiniteStaticSequence Intersect(Circle C, Line L) => Intersect(L, C);

  public static FiniteStaticSequence Intersect(Circle C, Ray R) => Intersect(R, C);

  public static FiniteStaticSequence Intersect(Circle C, Segment S)
      => Intersect(S, C);

  public static Sequence Intersect(Circle C1, Circle C2)
  {
    if (Equal_Vectors_Approx(C1.Center, C2.Center))
    {
      if (Equal_Approx(C1.Radius, C2.Radius))
        return new InfiniteStaticSequence();

      return new FiniteStaticSequence();
    }

    var k1 = C1.Center.X_Coord;
    var h1 = C1.Center.Y_Coord;
    var r1 = C1.Radius;

    var k2 = C2.Center.X_Coord;
    var h2 = C2.Center.Y_Coord;
    var r2 = C2.Radius;

    var A = 2 * (k2 - k1);
    var B = 2 * (h2 - h1);
    var C = -(r1 * r1 - r2 * r2 - k1 * k1 + k2 * k2 - h1 * h1 + h2 * h2);

    var Line = new Line(A, B, C);


    return Intersect(Line, C1);
  }

  public static Sequence Intersect(Circle C, Arc A)
  {
    var intersections = Intersect(C, new Circle(A.Center, A.Radius));

    if (intersections is FiniteStaticSequence)
    {
      var list = new List<Objects.GSObject>();
      foreach (var P in (FiniteStaticSequence)intersections)
      {
        if (Intersect((Point)P, A).Count > 0) list.Add(P);
      }

      return new FiniteStaticSequence(list);
    }

    return intersections;
  }

  #endregion

  #region Arc

  public static FiniteStaticSequence Intersect(Arc A, Point Point) => Intersect(Point, A);

  public static FiniteStaticSequence Intersect(Arc A, Line L) => Intersect(L, A);

  public static FiniteStaticSequence Intersect(Arc A, Ray R) => Intersect(R, A);

  public static FiniteStaticSequence Intersect(Arc A, Segment S)
      => Intersect(S, A);

  public static Sequence Intersect(Arc A, Circle C)
      => Intersect(C, A);

  public static Sequence Intersect(Arc A1, Arc A2)
  {
    var intersections = Intersect(new Circle(A1.Center, A1.Radius), A2);

    if (intersections is FiniteStaticSequence)
    {
      var list = new List<Objects.GSObject>();
      foreach (var P in (FiniteStaticSequence)intersections)
      {
        if (Intersect((Point)P, A1).Count > 0) list.Add(P);
      }

      return new FiniteStaticSequence(list);
    }


    return intersections;
  }

  #endregion

  #region Comparers

  public static int Approx_Compare_Double(double A, double B)
  {
    const double Epsilon = 1E-9;
    if (Math.Abs(A - B) <= Epsilon) return 0;
    if (A < B) return -1;
    // (A > B) 
    return 1;
  }

  public static bool Greater_Than_Approx(double A, double B) => Approx_Compare_Double(A, B) == 1;
  public static bool Less_Than_Approx(double A, double B) => Approx_Compare_Double(A, B) == -1;
  public static bool Equal_Approx(double A, double B) => Approx_Compare_Double(A, B) == 0;
  public static bool Equal_Vectors_Approx(Point A, Point B)
      => Equal_Approx(A.X_Coord, B.X_Coord) && Equal_Approx(A.Y_Coord, B.Y_Coord);
  public static bool Greater_Equal_Approx(double A, double B) => Approx_Compare_Double(A, B) >= 0;
  public static bool Less_Equal_Approx(double A, double B) => Approx_Compare_Double(A, B) <= 0;


  #endregion

}