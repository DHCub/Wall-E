namespace GSharp.Collections;
using System.Collections.Generic;
using Geometry;

// when this returns null upon indexing, it must be interpreted as reaching the infinite part
public partial class InfiniteStaticSequence<T> : Sequence<T>, IEnumerable<T>
{
    public int Prefix_Size { get { return base.Count; } }
    // the items in the base will be the prefix
    public InfiniteStaticSequence(ICollection<T> items = null) : base(items == null ? new List<T>() : items)
    { }

    public override InfiniteStaticSequence<T> GetRemainder(int start)
        => new InfiniteStaticSequence<T>(base.GetAllElementsFromStart(start));
}