using Godot;

using Geometry;
using GSharp;

using System;
using System.Collections.Generic;

public partial class Node2D : Godot.Node2D
{
    private const float PointRadius = 5;
    private List<(IDrawable drawable, Godot.Color color)> shapes = new();
    
    public override void _Ready()
    {
    
    }

    public override void _Draw()
    {
        var lineWidth = 2/Transform.X.X;

        var container = GetNode<Draw_Area_Marg>("../../../..");
        // DrawCircle(container.Size/2, PointRadius, Colors.Green);

        void draw_segment(Line L, Point P1, Point P2, Godot.Color color, bool P1_inf = false, bool P2_inf = false)
        {
            if (Functions.Equal_Vectors_Approx(P1, P2)) 
                throw new ArgumentException("Cannot Draw Line Passing Through Equal Points");
            
            if (Functions.Greater_Than_Approx(P1.X_Coord, P2.X_Coord)) 
            {
                draw_segment(L, P2, P1, color, P2_inf, P1_inf);
                return;
            }

            if (Functions.Equal_Approx(P1.X_Coord, P2.X_Coord))
            {
                if (Functions.Greater_Than_Approx(P1.Y_Coord , P2.Y_Coord))
                {
                    draw_segment(L, P2, P1, color, P2_inf, P1_inf);
                    return;
                }
            }

            // P1 is left, bottom, P2 is right, top

            double x1 = P1.X_Coord;
            double x2 = P2.X_Coord;
            
            double y1 = P1.Y_Coord;
            double y2 = P2.Y_Coord;
            
            double A = L.Normal_Vector.X_Coord;
            double B = L.Normal_Vector.Y_Coord;
            double C = L.Algebraic_Trace;

            double Window_X_Size = IDrawable.Window_EndX - IDrawable.Window_StartX;
            double Window_Y_Size = IDrawable.Window_EndY - IDrawable.Window_StartY;

            if (Functions.Greater_Than_Approx(Math.Abs(B), Math.Abs(A)))
            {
                if (P1_inf)
                {
                    y1 = IDrawable.Window_StartY - Window_Y_Size/2;
                    if (!Functions.Equal_Approx(A, 0)) x1 = -C/A - B/A*y1;
                }
                if (P2_inf)
                {
                    y2 = IDrawable.Window_EndY + Window_Y_Size/2;
                    if (!Functions.Equal_Approx(A, 0)) x2 = -C/A - B/A*y2;

                }
            }
            else
            {
                if (P1_inf)
                {
                    x1 = IDrawable.Window_StartX - Window_X_Size/2;
                    if (!Functions.Equal_Approx(B, 0)) y1 = -C/B - A/B*x1;
                }

                if (P2_inf)
                {
                    x2 = IDrawable.Window_EndX + Window_Y_Size/2;
                    if (!Functions.Equal_Approx(B, 0)) y2 = -C/B - A/B*x2;
                }
            }

            DrawLine(
                new Vector2((float)x1, (float)y1),
                new Vector2((float)x2, (float)y2),
                color,
                width: lineWidth,
                antialiased:true
            );
           
        }

        foreach(var (drawable, color) in shapes)
        {
            if (drawable is Point P)
            {
                DrawCircle((Vector2)P, PointRadius/Transform.X.X, color);
            }
            else if (drawable is Line L)
            {
                draw_segment(
                    L,
                    L.A_Point,
                    L.A_Point + L.Direction_Vector,
                    color,
                    true,
                    true
                );
            }
            else if (drawable is Ray R)
            {
                draw_segment(
                    Line.Point_DirectorVec(R.First_Point, R.Director_Vector),
                    R.First_Point,
                    R.First_Point + R.Director_Vector,
                    color,
                    false,
                    true
                );
            }
            else if (drawable is Segment S)
            {
                draw_segment(new Line(S.A_Point, S.B_Point), S.A_Point, S.B_Point, color);
            }
            else if (drawable is Circle Circle)
            {
                // DrawCircle(Circle.Center, (float)Circle.Radius, Circle.Color);
                DrawArc(
                    (Vector2)Circle.Center,
                    (float)Circle.Radius,
                    0,
                    (float)(2 * Math.PI),
                    10000,
                    color,
                    antialiased: true,
                    width: lineWidth);
            }
            else if (drawable is Arc Arc)
            {
                var start_angle = new Point(1, 0).AngleTo(Arc.Start_Ray.Director_Vector);


                DrawArc(
                    (Vector2)Arc.Center,
                    (float)Arc.Radius,
                    (float)start_angle,
                    (float)(start_angle + Arc.Angle),
                    10000,
                    color,
                    width:lineWidth,
                    true
                );
            }
            
        
        }


    }

    public void AddDrawable(Godot.Color color, params IDrawable[] drawable_array)
    {
        foreach(var drawable in drawable_array)
        {
            this.shapes.Add((drawable, color));
        }
    }

    public void Clear()
    {
        this.shapes.Clear();
        QueueRedraw();
    }        

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}

