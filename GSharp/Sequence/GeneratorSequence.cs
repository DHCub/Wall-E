namespace GSharp.Collections;

using System.Collections.Generic;
using System;
using System.Collections;
using Geometry;

public partial class GeneratorSequence<T> : InfiniteStaticSequence<T>, IEnumerable<T>
{
    Func<T> GeneratorFunction; // this must not return null
    public GeneratorSequence(Func<T> GeneratorFunction, ICollection<T> items = null) : base(items == null ? new List<T>() : items)
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

    public override GeneratorSequence<T> GetRemainder(int start)
    {
        var items = GetAllElementsFromStart(start);
        return new GeneratorSequence<T>(GeneratorFunction, items);
    }
}
