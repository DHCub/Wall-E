namespace GSharp.Collections;

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Geometry;

public abstract class Sequence<T> : IEnumerable<T>
{
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => this.items.GetEnumerator();
    protected List<T> items;

    protected int Count { get { return items.Count; } }

    public Sequence(ICollection<T> items)
    {
        this.items = items.ToList();
    }

#nullable enable

    public virtual T? this[int i]
    {
        get
        {
            if (i >= items.Count) return default;
            return items[i];
        }
    }

#nullable disable

    protected List<T> GetAllElementsFromStart(int start)
    {
        var answ = new List<T>();

        for (int i = start; i < items.Count; i++)
            answ.Add(items[i]);

        return answ;
    }

    public abstract Sequence<T> GetRemainder(int start);
}