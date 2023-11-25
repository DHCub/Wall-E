namespace Geometry;
using System;
using Godot;

public abstract class GeoExpr : IDrawable
{
    public static RandomNumberGenerator rnd = new();

    public abstract Point Sample();
}

public interface IDrawable
{
    public static float Window_StartX {get; private set;}
    public static float Window_EndX {get; private set;}
    public static float Window_StartY {get; private set;}
    public static float Window_EndY {get; private set;}

    public static float ZoomFactor {get; private set;}

    public const float Point_Representation_Radius = 5;

    public static void UpdateWindow(float startX, float endX, float startY, float endY, float zoomFactor)
    {
        Window_StartX = startX;
        Window_StartY = startY;
        Window_EndX = endX;
        Window_EndY = endY;
        ZoomFactor = zoomFactor;
    }
}

