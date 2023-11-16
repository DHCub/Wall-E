using Godot;
using System;
using System.Collections.Generic;

public class Node2D : Godot.Node2D
{
    private List<IDrawable> shapes = new List<IDrawable>();
    
    public override void _Ready()
    {
    
    }

    public override void _Draw()
    {
        DrawCircle(new Vector2(0, 0), 5, new Color(1000));

        Color c = new Color(100);
        foreach(var shape in shapes)
        {
            if (shape is Circle)
            {
                var Circle = (Circle)shape;

                // DrawCircle(Circle.Center, (float)Circle.Radius, Circle.Color);
                DrawArc(Circle.Center, (float)Circle.Radius, 0, (float)(2*Math.PI), 10000, c, antialiased:true);
            }
        }

    }

    public void Draw()
    {
        Update();
    }

    public void AddDrawable(IDrawable drawable)
    {
        this.shapes.Add(drawable);
    }

    public void Clear()
    {
        this.shapes.Clear();
        Update();
    }


    
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}