

namespace GSharp.GSObject.Figures;
using System;
using GSharp.GSObject.Collections;

public class RandomNumberGenerator
{
    private Random random;
    public RandomNumberGenerator(){random = new();}

    public double RandfRange(double from, double to)
    {
        var delta = (to - from)*random.NextDouble();

        return from + delta;
    }
}

public abstract class Figure : GSObject
{
    public static RandomNumberGenerator rnd = new();

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
    public abstract Point Sample();

    public override GSObject OperateScalar(Scalar other, Add op) => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, LessTh op) => UnsupportedOperation(other, op);

    public override GSObject OperateScalar(Scalar other, Mod op) => UnsupportedOperation(other, op);

    public override GSObject OperateScalar(Scalar other, Subst op) => UnsupportedOperation(other, op);

    public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op) => UnsupportedOperation(other, op);
    public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op) => UnsupportedOperation(other, op);
    public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op) => UnsupportedOperation(other, op);

    public override GSObject OperateUndefined(Undefined other, Add op)  => UnsupportedOperation(other, op);

    public override GSObject OperatePoint(Point other, Mult op) => UnsupportedOperation(other, op);

    public override GSObject OperateString(GSharp.GSObject.String other, Add op) => UnsupportedOperation(other, op);

    public override bool GetTruthValue() => true;

}
