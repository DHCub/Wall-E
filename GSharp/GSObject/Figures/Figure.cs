

namespace GSharp.Objects.Figures;
using System;
using GSharp.Objects.Collections;

public class RandomNumberGenerator
{
    private Random random;
    public RandomNumberGenerator() { random = new(); }

    public double RandDoubleRange(double from, double to)
    {
        var delta = (to - from) * random.NextDouble();

        return from + delta;
    }
}

public abstract class Figure : GSObject
{
    public static RandomNumberGenerator rnd = new();

    public static double WindowStartX { get; private set; }
    public static double WindowEndX { get; private set; }
    public static double WindowStartY { get; private set; }
    public static double WindowEndY { get; private set; }

    public static double ZoomFactor { get; private set; }

    public const double PointRepresentationRadius = 5;

    public static void UpdateWindow(double startX, double endX, double startY, double endY, double zoomFactor)
    {
        WindowStartX = startX;
        WindowStartY = startY;
        WindowEndX = endX;
        WindowEndY = endY;
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

    public override GSObject OperateUndefined(Undefined other, Add op) => UnsupportedOperError(other, op);

    public override GSObject OperatePoint(Point other, Mult op) => UnsupportedOperError(other, op);

    public override GSObject OperateMeasure(Measure other, Add op) => UnsupportedOperError(other, op);
    public override GSObject OperateMeasure(Measure other, Subst op) => UnsupportedOperError(other, op);
    public override GSObject OperateMeasure(Measure other, Mod op) => UnsupportedOperError(other, op);
    public override GSObject OperateMeasure(Measure other, LessTh op) => UnsupportedOperError(other, op);

    public override GSObject OperateString(GSharp.Objects.String other, Add op) => UnsupportedOperError(other, op);

    public override bool GetTruthValue() => true;

}

