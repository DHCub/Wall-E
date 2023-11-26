// when this returns null upon indexing, it must be interpreted as surpassing the end
namespace GSharp;

using System.Collections.Generic;
using Geometry;

public partial class Finite_Static_Seqence<T> : Sequence<T>, IEnumerable<T> where T: class
{
    public new int Count { get { return base.Count; } }

    public Finite_Static_Seqence(ICollection<T> items = null) : base(items == null ? new List<T>() : items)
    { }

    public override Finite_Static_Seqence<T> GetRemainder(int start)
        => new Finite_Static_Seqence<T>(base.GetAllElementsFromStart(start));
}