using Godot;
using System;
using System.Collections.Generic;

public class Node2D : Godot.Node2D
{
    public List<Shape> shapes = new List<Shape>();
    
    public override void _Ready()
    {
    
    }

    public override void _Draw()
    {
        foreach(var shape in shapes)
        {
            if (shape is Circle)
            {
                var Circle = (Circle)shape;

                DrawCircle(Circle.pos, Circle.Radius, Circle.color);
            }
        }
    }

    public void draw()
    {
        Update();
    }

    
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

}

public abstract class Shape
{
}

public class Circle : Shape
{
    public Vector2 pos;
    public Color color;
    public float Radius;

    public Circle(Vector2 pos, float Radius, Color color)
    {
        this.pos = pos;
        this.color = color;
        this.Radius = Radius;
    }
}
