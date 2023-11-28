// when this returns null upon indexing, it must be interpreted as surpassing the end
namespace GSharp.Collections;

using System.Collections.Generic;
using Geometry;

public partial class FiniteStaticSequence<T> : Sequence<T>, IEnumerable<T>
{
    public new int Count { get { return base.Count; } }

    public FiniteStaticSequence(ICollection<T> items = null) : base(items == null ? new List<T>() : items)
    { }

    public override FiniteStaticSequence<T> GetRemainder(int start)
        => new FiniteStaticSequence<T>(base.GetAllElementsFromStart(start));
}