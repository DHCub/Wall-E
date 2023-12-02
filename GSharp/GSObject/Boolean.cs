using GSharp.GSObject.Collections;
using GSharp.GSObject.Figures;

namespace GSharp.GSObject;

public class Boolean : GSObject
{
    public readonly bool value;
    public Boolean(bool value)
    {
        this.value = value;
    }

    public override bool Equals(GSObject obj) => obj is Boolean b && b.value == this.value;

    public override bool GetTruthValue() => this.value;

    public override string GetTypeName() => GSTypes.Boolean.ToString();

    public override string ToString() => this.value.ToString();

    public override GSObject OperateUndefined(Undefined other, Add op) => UnsupportedOperation(other, op);

    public override GSObject OperateString(String other, Add op) => UnsupportedOperation(other, op);


    public override GSObject OperateScalar(Scalar other, Add op) => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, Subst op) => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, Mult op) => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, Div op) => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, Mod op) => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, LessTh op) => UnsupportedOperation(other, op);


    public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op) => UnsupportedOperation(other, op);
    public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op) => UnsupportedOperation(other, op);
    public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op) => UnsupportedOperation(other, op);


    public override GSObject OperatePoint(Point other, Add op) => UnsupportedOperation(other, op);
    public override GSObject OperatePoint(Point other, Subst op) => UnsupportedOperation(other, op);
    public override GSObject OperatePoint(Point other, Mult op) => UnsupportedOperation(other, op);

} 