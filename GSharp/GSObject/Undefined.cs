using GSharp.GSObject.Collections;
using GSharp.GSObject.Figures;

namespace GSharp.GSObject;

public class Undefined : GSObject
{

    public override bool Equals(GSObject obj) => obj is Undefined;

    public override bool GetTruthValue() => false;

    public override string GetTypeName() => UNDEFINED;

    public override string ToString() => UNDEFINED;

    public override GSObject OperateString(String other, Add op) => UnsupportedOperation(other, op);



    public override GSObject OperateScalar(Scalar other, Add op) => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, Subst op) => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, Mult op) => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, Div op) => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, Mod op) => UnsupportedOperation(other, op);
    public override GSObject OperateScalar(Scalar other, LessTh op) => UnsupportedOperation(other, op);


    public override GSObject OperatePoint(Point other, Add op) => UnsupportedOperation(other, op);
    public override GSObject OperatePoint(Point other, Subst op) => UnsupportedOperation(other, op);
    public override GSObject OperatePoint(Point other, Mult op) => UnsupportedOperation(other, op);
    
    
    public override GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op) => new InfiniteStaticSequence();
    public override GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op) => new InfiniteStaticSequence();
    public override GSObject OperateGeneratorSequence(GeneratorSequence other, Add op) => new InfiniteStaticSequence();

    public override GSObject OperateUndefined(Undefined other, Add op) => UnsupportedOperation(other, op);
}

