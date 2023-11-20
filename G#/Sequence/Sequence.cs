namespace GSharp;

using System.Collections.Generic;
using System.Collections;
using System.Linq;

public abstract class Sequence<T> : IEnumerable<T> where T : class
{
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => this.items.GetEnumerator();
    protected List<T> items;

    protected int Count { get { return items.Count; } }

    public Sequence(ICollection<T> items)
    {
        this.items = items.ToList();
    }

    public virtual T this[int i]
    {
        get
        {
            if (i >= items.Count) return null;
            return items[i];
        }
    }

    protected List<T> GetAllElementsFromStart(int start)
    {
        var answ = new List<T>();

        for (int i = start; i < items.Count; i++)
            answ.Add(items[i]);

        return answ;
    }

    public abstract Sequence<T> GetRemainder(int start);
}