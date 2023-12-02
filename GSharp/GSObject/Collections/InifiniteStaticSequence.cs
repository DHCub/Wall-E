namespace GSharp.Objects.Collections;
using System.Collections.Generic;
using System.Linq;
using Figures;

// when this returns null upon indexing, it must be interpreted as reaching the infinite part
public partial class InfiniteStaticSequence : Sequence
{
    private readonly List<GSObject> prefix;
    public InfiniteStaticSequence(ICollection<GSObject> items = null)
    {
        if (items == null) prefix = new();
        else prefix = items.ToList();
    }
    public override IEnumerable<GSObject> GetPrefixValues() => prefix;

    public override string ToString() => INFINITE_SEQUENCE;
    public override bool Equals(GSObject obj) => false;
    public override bool GetTruthValue() => true;

    public override GSObject GSCount() => new Undefined();

    public override Sequence GetRemainder(int start)
    {
        List<GSObject> newItems = new();

        for(int i = start; i < prefix.Count; i++)
        {
            newItems.Add(prefix[i]);
        }

        return new InfiniteStaticSequence(newItems);
    }

    public override GSObject this[int i] => (i >= prefix.Count)? new Undefined() : prefix[i];


    public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op)
        => new InfiniteStaticSequence(this.prefix);

    public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op)
        => new InfiniteStaticSequence(this.prefix);
    public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op)
        => new InfiniteStaticSequence(this.prefix);

    public override GSObject OperateUndefined(Undefined other, Add op)
        => new InfiniteStaticSequence(this.prefix);
}