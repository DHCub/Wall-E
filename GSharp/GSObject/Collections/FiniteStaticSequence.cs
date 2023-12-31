using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GSharp.Objects.Collections;

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Figures;

public partial class FiniteStaticSequence : Sequence, IEnumerable<GSObject>
{
  private List<GSObject> items;
  public override int PrefixLength() => items.Count;
  public FiniteStaticSequence(ICollection<GSObject> items = null)
  {
    if (items == null) this.items = new();
    else this.items = items.ToList();
  }

  public override GSObject GSCount() => (Scalar)items.Count;
  public override IEnumerable<GSObject> GetPrefixValues() => this.items;
  public int Count => items.Count;

  public IEnumerator<GSObject> GetEnumerator() => items.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

  public override bool Equals(GSObject obj)
  {
    if (obj is FiniteStaticSequence finSeq)
    {
      for (int i = 0; i < finSeq.Count; i++)
      {
        if (!this[i].Equals(finSeq[i])) return false;
      }

      return true;
    }

    return false;
  }

  public override bool SameTypeAs(GSObject gso)
  {
    if (gso is Sequence && this.Count == 0) return true;

    // Count != 0 if any of these ifs are entered

    if (gso is FiniteStaticSequence finSeq)
    {
      if (finSeq.Count == 0) return true;
      return this[0].SameTypeAs(finSeq[0]);
    }

    if (gso is InfiniteStaticSequence infseq)
    {
      if (infseq.PrefixLength() == 0) return true;
      return this[0].SameTypeAs(infseq[0]);
    }

    if (gso is GeneratorSequence genSeq)
    {
      return this[0].SameTypeAs(genSeq[0]);
    }

    return false;
  }

  public override string ToString()
  {
    List<char> answ = new();
    answ.Add('{');

    for (int i = 0; i < this.Count - 1; i++)
    {
      answ.AddRange(items[i].ToString());
      answ.AddRange(", ");
    }

    if (items.Count > 0)
      answ.AddRange(items[Count - 1].ToString());

    answ.Add('}');

    return new string(answ.ToArray());
  }

  public override GSObject this[int i] { get => (i >= Count) ? new Undefined() : items[i]; }

  public override bool GetTruthValue() => this.Count != 0;
  public override Sequence GetRemainder(int start)
  {
    List<GSObject> newItems = new();

    for (int i = start; i < this.Count; i++)
      newItems.Add(items[i]);

    return new FiniteStaticSequence(newItems);
  }

  public override GSObject OperateUndefined(Undefined other, Add op)
      => new InfiniteStaticSequence(this.items);

  public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op)
  {
    List<GSObject> newItems = this.items.ToList();

    foreach (var item in other)
      newItems.Add(item);

    return new FiniteStaticSequence(newItems);
  }

  public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op)
  {
    List<GSObject> newItems = items.ToList();

    foreach (var prefixItem in other.GetPrefixValues())
      newItems.Add(prefixItem);

    return new InfiniteStaticSequence(newItems);
  }

  public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op)
  {
    List<GSObject> newItems = this.items.ToList();

    foreach (var prefixItem in other.GetPrefixValues())
      newItems.Add(prefixItem);

    return new GeneratorSequence(other.generator, newItems);
  }
}