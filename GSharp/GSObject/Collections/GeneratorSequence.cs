namespace GSharp.Objects.Collections;

using System.Collections.Generic;
using System;
using System.Collections;
using Figures;
using System.Linq;

public class GeneratorSequence : Sequence
{
    public readonly GSGenerator generator;
    private readonly List<GSObject> prefix;

    public GeneratorSequence(GSGenerator generator, ICollection<GSObject> items = null)
    {
        if (items == null) prefix = new();
        else prefix = items.ToList();

        this.generator = generator.GetCopy();
    }

    public override IEnumerable<GSObject> GetPrefixValues() => prefix;

    public override bool Equals(GSObject obj)
    {
        if (obj is GeneratorSequence genSeq)
        {
            // if the amounts in the prefixes are different we generate the remaining ones in the smaller one
            if (genSeq.prefix.Count > this.prefix.Count)
                _ = this[genSeq.prefix.Count - 1];
            else if (this.prefix.Count > genSeq.prefix.Count)
                _ = genSeq[this.prefix.Count - 1];

            if (!genSeq.generator.Equals(this.generator)) return false;

            for(int i = 0; i < this.prefix.Count; i++)
            {
                if (!this.prefix[i].Equals(genSeq.prefix[i])) return false;
            }

            return true;
        }

        return false;
    }

    public override GSObject GSCount() => new Undefined();

    public override string ToString() => INFINITE_SEQUENCE;

    public override bool GetTruthValue() => true;

    public override Sequence GetRemainder(int start)
    {
        List<GSObject> newItems = new();

        if (start > this.prefix.Count) _ = this.prefix[start - 1];

        for (int i = start; i < this.prefix.Count; i++)
        {
            newItems.Add(prefix[i]);
        }

        return new GeneratorSequence(this.generator, newItems);
    }

    public override GSObject this[int i]
    {
        get{
            if (i > this.prefix.Count)
                for (int j = this.prefix.Count; j <= i; j++)
                    this.prefix.Add(generator.GetNextValue());
            
            return prefix[i];
        }
    }

    public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op)
        => new GeneratorSequence(generator, prefix);

    public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op)
        => new GeneratorSequence(generator, prefix);

    public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op)
        => new GeneratorSequence(generator, prefix);


    public override GSObject OperateUndefined(Undefined other, Add op)
        => new GeneratorSequence(generator, prefix);
}



public abstract class GSGenerator
{
    public abstract GSGenerator GetCopy();
    public abstract GSObject GetNextValue();
    public abstract bool Equals(GSGenerator other);
}

public class IntRangeGenerator : GSGenerator
{
    private int current;

    public IntRangeGenerator(int start)
    {
        this.current = start;
    }

    public override GSObject GetNextValue() => new Scalar(current++);
    public override bool Equals(GSGenerator other)
    {
        if (other is IntRangeGenerator IRG)
        {
            if (IRG.current == this.current) return true;
        }

        return false;
    }
    public override GSGenerator GetCopy() => new IntRangeGenerator(current);
}

public class RandomPointInFigureGenerator : GSGenerator
{
    private readonly InternalPointInFigureGenerator @internal;
    private int current;
    
    public RandomPointInFigureGenerator(Figure figure)
    {
        @internal = new(figure);
        current = 0;
    }

    private RandomPointInFigureGenerator(RandomPointInFigureGenerator other)
    {
        this.current = other.current;
        this.@internal = other.@internal;
    }

    public override bool Equals(GSGenerator other)
        => other is RandomPointInFigureGenerator gen && 
           gen.@internal == this.@internal && 
           gen.current == this.current;

    public override GSGenerator GetCopy() => new RandomPointInFigureGenerator(this);

    public override GSObject GetNextValue()
        => @internal[current++];
}

public class RandomDoubleGenerator : GSGenerator
{
    private int current;
    private InternalDoubleGenerator @internal;

    public RandomDoubleGenerator()
    {
        @internal = new();
        current = 0;
    }

    private RandomDoubleGenerator(RandomDoubleGenerator other)
    {
        this.current = other.current;
        this.@internal = other.@internal;
    }

    public override bool Equals(GSGenerator other)
        => other is RandomDoubleGenerator RDG &&
            RDG.@internal == this.@internal &&
            RDG.current == this.current;

    public override GSObject GetNextValue()
        => @internal[current++];

    public override GSGenerator GetCopy()
        => new RandomDoubleGenerator(this);

}

public class RandomPointInCanvasGenerator : GSGenerator
{
    private int current;
    private InternalPointInCanvasGenerator @internal;

    public RandomPointInCanvasGenerator()
    {
        current = 0;
        @internal = new();
    }

    private RandomPointInCanvasGenerator(RandomPointInCanvasGenerator other)
    {
        this.current = other.current;
        this.@internal = other.@internal;
    }

    public override bool Equals(GSGenerator other)
        => other is RandomPointInCanvasGenerator RPICG &&
            RPICG.@internal == this.@internal &&
            RPICG.current == this.current;

    public override GSGenerator GetCopy()
        => new RandomPointInCanvasGenerator(this);

    public override GSObject GetNextValue() => @internal[current++];

}

public enum FigureOptions
{
    Point,
    Line,
    Ray,
    Segment,
    Circle,
    Arc
}

public class RandomFigureGenerator : GSGenerator
{
    private int current;
    private InternalRandomFigureGenerator @internal;
    public RandomFigureGenerator(FigureOptions figure)
    {
        @internal = new(figure);
        current = 0;
    }
    
    private RandomFigureGenerator(RandomFigureGenerator other)
    {
        current = other.current;
        @internal = other.@internal;
    }

    public override bool Equals(GSGenerator other)
        => other is RandomFigureGenerator RFG && 
        RFG.current == this.current &&
        RFG.@internal == this.@internal;

    public override GSObject GetNextValue()
        => @internal[current++];

    public override GSGenerator GetCopy()
        => new RandomFigureGenerator(this);
}

abstract class InternalGenerator
{
    public abstract GSObject this[int i] {get;}
}

class InternalPointInFigureGenerator : InternalGenerator
{
    private readonly Figure figure;
    private List<Point> generatedPoints;

    public InternalPointInFigureGenerator(Figure figure)
    {
        this.figure = figure;
        this.generatedPoints = new();
    }

    public override GSObject this[int i]
    {
        get{
            if (i >= generatedPoints.Count)
                for (int j = generatedPoints.Count; j <= i; j++)
                    generatedPoints.Add(figure.Sample());

            return generatedPoints[i];
        }
    }
} 

class InternalDoubleGenerator : InternalGenerator
{
    private readonly Random random;
    private List<double> generatedDoubles;
    public override GSObject this[int i]
    {
        get{
            if (i >= generatedDoubles.Count)
                for (int j = generatedDoubles.Count; j <= i; j++)
                    generatedDoubles.Add(random.NextDouble());

            return new Scalar(generatedDoubles[i]);
        }
    }
}

class InternalPointInCanvasGenerator: InternalGenerator
{
    private List<Point> generatedPoints;
    public InternalPointInCanvasGenerator()
    {
        generatedPoints = new();
    }

    public override GSObject this[int i]
    {
        get{
            if (i > generatedPoints.Count)
                for (int j = generatedPoints.Count; j <= i; j++)
                    generatedPoints.Add(new Point());

            return generatedPoints[i];
        }
    }
}

class InternalRandomFigureGenerator : InternalGenerator
{
    private List<Figure> generatedFigures;
    private FigureOptions figure;

    public InternalRandomFigureGenerator(FigureOptions figure)
    {
        this.figure = figure;
    }

    public override GSObject this[int i]
    {
        get{
            if (i > this.generatedFigures.Count)
                for (int j = generatedFigures.Count; j < i; j++)
                    generatedFigures.Add(
                        figure switch
                        {
                            FigureOptions.Point => new Point(),
                            FigureOptions.Line => new Line(),
                            FigureOptions.Ray => new Ray(),
                            FigureOptions.Segment => new Segment(),
                            FigureOptions.Circle => new Circle(),
                            FigureOptions.Arc => new Arc(),
                            _ => throw new NotImplementedException("UNSUPPORTED FIGURE TYPE FOR GENERATION"),
                        }
                    );
            
            return generatedFigures[i];
        }
    }
}
