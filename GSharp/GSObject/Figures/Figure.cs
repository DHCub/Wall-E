

namespace GSharp.Objects.Figures;
using System;
using GSharp.Objects.Collections;

public class RandomNumberGenerator
{
    private Random random;
    public RandomNumberGenerator(){random = new();}

    public double RandDoubleRange(double from, double to)
    {
        var delta = (to - from)*random.NextDouble();

        return from + delta;
    }
}

public abstract class Figure : GSObject
{
    public static RandomNumberGenerator rnd = new();

    public static double Window_StartX {get; private set;}
    public static double Window_EndX {get; private set;}
    public static double Window_StartY {get; private set;}
    public static double Window_EndY {get; private set;}

    public static double ZoomFactor {get; private set;}

    public const double Point_Representation_Radius = 5;

    public static void UpdateWindow(double startX, double endX, double startY, double endY, double zoomFactor)
    {
        Window_StartX = startX;
        Window_StartY = startY;
        Window_EndX = endX;
        Window_EndY = endY;
        ZoomFactor = zoomFactor;
    }
    public abstract Point Sample();

    public override GSObject OperateScalar(Scalar other, Add op) => UnsupportedOperError(other, op);
    public override GSObject OperateScalar(Scalar other, LessTh op) => UnsupportedOperError(other, op);

    public override GSObject OperateScalar(Scalar other, Mod op) => UnsupportedOperError(other, op);

    public override GSObject OperateScalar(Scalar other, Subst op) => UnsupportedOperError(other, op);

    public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op) => UnsupportedOperError(other, op);
    public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op) => UnsupportedOperError(other, op);
    public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op) => UnsupportedOperError(other, op);

    public override GSObject OperateUndefined(Undefined other, Add op)  => UnsupportedOperError(other, op);

    public override GSObject OperatePoint(Point other, Mult op) => UnsupportedOperError(other, op);

    public override GSObject OperateMeasure(Measure other, Add op) => UnsupportedOperError(other, op);
    public override GSObject OperateMeasure(Measure other, Subst op) => UnsupportedOperError(other, op);
    public override GSObject OperateMeasure(Measure other, Mod op) => UnsupportedOperError(other, op);
    public override GSObject OperateMeasure(Measure other, LessTh op) => UnsupportedOperError(other, op);

    public override GSObject OperateString(GSharp.Objects.String other, Add op) => UnsupportedOperError(other, op);

    public override bool GetTruthValue() => true;

}

