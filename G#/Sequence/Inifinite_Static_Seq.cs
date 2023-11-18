namespace GSharp;
using System.Collections.Generic;
using Geometry;

// when this returns null upon indexing, it must be interpreted as reaching the infinite part
public partial class Infinite_Static_Sequence<T> : Sequence<T>, IEnumerable<T> where T: GeoExpr
{
    public int Prefix_Size {get{return base.Count;}}
    // the items in the base will be the prefix
    public Infinite_Static_Sequence(ICollection<T> items = null) : base(items == null ? new List<T>() : items)
    {}

    public override Infinite_Static_Sequence<T> GetRemainder(int start)
        => new Infinite_Static_Sequence<T>(base.GetAllElementsFromStart(start));
}