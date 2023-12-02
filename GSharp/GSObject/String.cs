using GSharp.GSObject.Collections;
using GSharp.GSObject.Figures;

namespace GSharp.GSObject;

public class String : GSObject
{
    public readonly string value;

    public String(string value)
    {
        this.value = value;
    }

    public override string ToString()
        => $"\"{this.value}\"";

    public override string GetTypeName()
        => GSTypes.String.ToString();

    public override bool GetTruthValue()
        => true;

    public override bool Equals(GSObject obj) => obj is String s && s.value == this.value;

    public override GSObject OperateString(String other, Add op) => new String(this.value + other.value);


    public override GSObject OperateScalar(Scalar other, Add op)
        => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, Subst op)
        => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, Mult op)
        => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, Div op)
        => UnsupportedOperation(other, op);

    public override GSObject OperateScalar(Scalar other, Mod op)
        => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, LessTh op)
        => UnsupportedOperation(other, op);

    
    
    public override GSObject OperatePoint(Point other, Add op)
        => UnsupportedOperation(other, op);

    public override GSObject OperatePoint(Point other, Mult op)
        => UnsupportedOperation(other, op);

    public override GSObject OperatePoint(Point other, Subst op)
        => UnsupportedOperation(other, op);

   
    public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op)
        => UnsupportedOperation(other, op);
   
    public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op)
        => UnsupportedOperation(other, op);

   
    public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op)
        => UnsupportedOperation(other, op);

    public override GSObject OperateUndefined(Undefined other, Add op) => UnsupportedOperation(other, op);

}