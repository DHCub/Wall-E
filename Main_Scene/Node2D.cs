using Godot;

using GSharp;
using GSharp.Objects.Figures;

using System;
using System.Collections.Generic;

public partial class Node2D : Godot.Node2D
{
    private List<(Figure drawable, Godot.Color color, string? label, Point labelLoc)> shapes = new();

    private bool ShowAxes;

    public void ToggleAxes() 
    {
        ShowAxes = !ShowAxes;
        QueueRedraw();
    }
    
    public override void _Ready()
    {
    
    }

    public override void _Draw()
    {
        var lineWidth = 2/Transform.X.X;
        const double Point_Representation_Radius = 5;
        var axesVectorMultiplier = GetNode<Draw_Area_Marg>("../../../..").axesVectorMultiplier;


        // DrawCircle(container.Size/2, PointRadius, Colors.Green);
        Godot.Vector2 GetVect2(Point P) {
            return new Godot.Vector2((float)P.XCoord, (float)P.YCoord);
        }

        void draw_segment(Line L, Point P1, Point P2, Godot.Color color, bool P1_inf = false, bool P2_inf = false)
        {
            // if (Functions.EqualVectorsApprox(P1, P2)) 
            //     throw new ArgumentException("Cannot Draw Line Passing Through Equal Points");
            
            if (Functions.GreaterThanApprox(P1.XCoord, P2.XCoord)) 
            {
                draw_segment(L, P2, P1, color, P2_inf, P1_inf);
                return;
            }

            if (Functions.EqualApprox(P1.XCoord, P2.XCoord))
            {
                if (Functions.GreaterThanApprox(P1.YCoord , P2.YCoord))
                {
                    draw_segment(L, P2, P1, color, P2_inf, P1_inf);
                    return;
                }
            }

            // P1 is left, bottom, P2 is right, top

            double x1 = P1.XCoord;
            double x2 = P2.XCoord;
            
            double y1 = P1.YCoord;
            double y2 = P2.YCoord;
            
            double A = L.NormalVector.XCoord;
            double B = L.NormalVector.YCoord;
            double C = L.AlgebraicTrace;

            double Window_X_Size = Figure.WindowEndX - Figure.WindowStartX;
            double Window_Y_Size = Figure.WindowEndY - Figure.WindowStartY;

            if (Functions.GreaterThanApprox(Math.Abs(A), Math.Abs(B)))
            {
                if (P1_inf)
                {
                    y1 = Figure.WindowStartY - Window_Y_Size/2;
                    x1 = -C/A - B/A*y1;
                }
                if (P2_inf)
                {
                    y2 = Figure.WindowEndY + Window_Y_Size/2;
                    x2 = -C/A - B/A*y2;
                }
            }
            else
            {
                if (P1_inf)
                {
                    x1 = Figure.WindowStartX - Window_X_Size/2;
                    if (!Functions.EqualApprox(B, 0)) y1 = -C/B - A/B*x1;
                }

                if (P2_inf)
                {
                    x2 = Figure.WindowEndX + Window_Y_Size/2;
                    if (!Functions.EqualApprox(B, 0)) y2 = -C/B - A/B*x2;
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

        void show_axes()
        {
            var X = new Line(new(0, 0), new(1, 0));
            var Y = new Line(new(0, 0), new(0, 1));

            draw_segment(X, new(0, 0), new(1, 0), Colors.Black, true, true);
            draw_segment(Y, new(0, 0), new(0, 1), Colors.Black, true, true);
        }


        // foreach(var child in this.GetChildren())
        //     if (child is Label) this.RemoveChild(child);

        if (ShowAxes) show_axes();

        for(int i = 0; i < shapes.Count; i++)
        {
            var (drawable, color, label, labelLoc) = shapes[i];
            if (label != null) 
            {
                if (labelLoc == null)
                {
                    labelLoc = drawable.Sample();
                    shapes[i] = (drawable, color, label, labelLoc);
                }
                DrawSetTransform(GetVect2(labelLoc), 0, new(1/axesVectorMultiplier, -1/axesVectorMultiplier));
                DrawString(ThemeDB.FallbackFont, GetVect2(labelLoc), label, modulate: Colors.Black);
                DrawSetTransform(Vector2.Zero, 0, Vector2.One);
            }

            if (drawable is Point P)
            {
                DrawCircle(GetVect2(P), (float)Point_Representation_Radius/Transform.X.X, color);
            }
            else if (drawable is Line L)
            {
                draw_segment(
                    L,
                    L.APoint,
                    L.APoint + L.DirectorVector,
                    color,
                    true,
                    true
                );
            }
            else if (drawable is Ray R)
            {
                draw_segment(
                    Line.PointDirectorVec(R.FirstPoint, R.DirectorVector),
                    R.FirstPoint,
                    R.FirstPoint + R.DirectorVector,
                    color,
                    false,
                    true
                );
            }
            else if (drawable is Segment S)
            {
                draw_segment(new Line(S.APoint, S.BPoint), S.APoint, S.BPoint, color);
            }
            else if (drawable is Circle Circle)
            {
                // DrawCircle(Circle.Center, (float)Circle.Radius, Circle.Color);
                DrawArc(
                    GetVect2(Circle.Center),
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
                var start_angle = new Point(1, 0).AngleTo(Arc.Start_Ray.DirectorVector);


                DrawArc(
                    GetVect2(Arc.Center),
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

    public void AddDrawable(Godot.Color color, params Figure[] drawable_array)
    {
        foreach(var drawable in drawable_array)
        {
            this.shapes.Add((drawable, color, null, null));
        }
    }

    public void AddDrawable(Color color, Figure figure, string label)
    {
        this.shapes.Add((figure, color, label, null));
    }

    public void Clear()
    {
        this.shapes.Clear();
        foreach(var child in this.GetChildren())
            if (child is Label) this.RemoveChild(child);
        QueueRedraw();
    }        

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}

