using System.Collections.Generic;
using GSharp.Objects.Figures;

namespace GSharp.Objects.Collections;

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using GSharp.Objects.Figures;
using System;
using GSharp.Types;
using GSharp.Exceptions;

public abstract class Sequence : GSObject
{    
    protected const string INFINITE_SEQUENCE = "INFINITE SEQUENCE";
    public abstract Sequence GetRemainder(int start);
    public abstract GSObject this[int i] {get;}
    public abstract GSObject GSCount();
    public abstract int PrefixLength();
    public override string GetTypeName() => $"Seq<{this[0].GetTypeName()}>";
    public override bool SameTypeAs(GSType gst) => gst.SameTypeAs(new SequenceType());
    public abstract IEnumerable<GSObject> GetPrefixValues();
    public override GSObject OperateScalar(Scalar other, Add op) => UnsupportedOperError(other, op);
    public override GSObject OperateScalar(Scalar other, Subst op) => UnsupportedOperError(other, op);
    public override GSObject OperateScalar(Scalar other, Mult op) => UnsupportedOperError(other, op);
    public override GSObject OperateScalar(Scalar other, Div op) => UnsupportedOperError(other, op);
    public override GSObject OperateScalar(Scalar other, Mod op) => UnsupportedOperError(other, op);
    public override GSObject OperateScalar(Scalar other, LessTh op) => UnsupportedOperError(other, op);
    public override GSObject OperateScalar(Scalar other, Indexer op)
    {
      var (isInteger, i) = Functions.GetInteger(other);
      if (!isInteger || i < 0) throw new RuntimeError(null, IndexingValueMustBeNonNegativeInteger(other), null);
      return this[i]; 
    }

  public override GSObject OperateMeasure(Measure other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Subst op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Mult op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Div op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, Mod op) => UnsupportedOperError(other, op);
  public override GSObject OperateMeasure(Measure other, LessTh op) => UnsupportedOperError(other, op);


  public override GSObject OperatePoint(Point other, Add op) => UnsupportedOperError(other, op);
  public override GSObject OperatePoint(Point other, Subst op) => UnsupportedOperError(other, op);
  public override GSObject OperatePoint(Point other, Mult op) => UnsupportedOperError(other, op);

  public override GSObject OperateString(GSharp.Objects.String other, Add op) => UnsupportedOperError(other, op);
}