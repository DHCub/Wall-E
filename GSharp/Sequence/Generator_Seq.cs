namespace GSharp;

using System.Collections.Generic;
using System;
using System.Collections;
using Geometry;

public partial class Generator_Sequence<T> : Infinite_Static_Sequence<T>, IEnumerable<T> where T:GeoExpr
{
    Func<T> GeneratorFunction; // this must not return null
    public Generator_Sequence(Func<T> GeneratorFunction, ICollection<T> items = null) : base(items == null ? new List<T>() : items)
    {
        this.GeneratorFunction = GeneratorFunction;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        int i = 0;
        while (true)
            yield return this[i++];
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        int i = 0;
        while (true)
            yield return this[i++];
    }


    public override T this[int i]
    {
        get
        {
            if (i >= base.items.Count)
            {
                for (int k = items.Count; k <= i; k++)
                {
                    items.Add(GeneratorFunction());
                }
            }

            return base[i];
        }
    }

    public override Generator_Sequence<T> GetRemainder(int start)
    {
        var items = GetAllElementsFromStart(start);
        return new Generator_Sequence<T>(GeneratorFunction, items);
    }
}
