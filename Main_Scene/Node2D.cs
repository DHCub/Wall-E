using Godot;
using System;
using System.Collections.Generic;
using Geometry;

public partial class Node2D : Godot.Node2D
{
    private List<(GeoExpr geoExpr, Color color)> shapes = new List<(GeoExpr geoExpr, Color color)>();
    
    public override void _Ready()
    {
    
    }

    public override void _Draw()
    {

        foreach(var shape in shapes)
        {
            if (shape.geoExpr is Circle)
            {
                var Circle = (Circle)shape.geoExpr;

                // DrawCircle(Circle.Center, (float)Circle.Radius, Circle.Color);
                DrawArc((Vector2)Circle.Center, (float)Circle.Radius, 0, (float)(2*Math.PI), 10000, shape.color, antialiased:true, width: 2);
            }
        }

    }

    public void ReDraw()
    {
        this.QueueRedraw();
    }

    public void AddDrawable(GeoExpr drawable, Color color)
    {
        this.shapes.Add((drawable, new Color(1000)));
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

