namespace GSharp;
using System;
using Objects;
using Objects.Figures;
using Objects.Collections;
public interface IOperate<OP> where OP: Operator
{
    public static GSObject Operate(IOperate<OP> operable1, IOperate<OP> operable2)
    {
        if (operable2 is Point P) return operable1.OperatePoint(P, null);
        if (operable2 is Line L) return operable1.OperateLine(L, null);
        if (operable2 is Segment S) return operable1.OperateSegment(S, null);
        if (operable2 is Circle C) return operable1.OperateCircle(C, null);
        if (operable2 is Arc A) return operable1.OperateArc(A, null);
        
        
        if (operable2 is Scalar scalar) return operable1.OperateScalar(scalar, null);
        if (operable2 is Measure measure) return operable1.OperateMeasure(measure, null);
        if (operable2 is Objects.String str) return operable1.OperateString(str, null);
        if (operable2 is Undefined u) return operable1.OperateUndefined(u, null);

        throw new NotImplementedException("GSOBJECT UNSUPPORTED");
    }

    public GSObject UnsupportedOperError(Objects.GSObject other, OP op);

    public GSObject OperatePoint(Point other, OP op);
    public GSObject OperateLine(Line other, OP op);
    public GSObject OperateSegment(Segment other, OP op);
    public GSObject OperateRay(Ray other, OP op);
    public GSObject OperateCircle(Circle other, OP op);
    public GSObject OperateArc(Arc other, OP op);
    
    
    public GSObject OperateScalar(Scalar other, OP op);
    public GSObject OperateMeasure(Measure other, OP op);
    
    
    public GSObject OperateString(Objects.String other, OP op);
    
    
    public GSObject OperateUndefined(Undefined other, OP op);
    
    
    public GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, OP op);
    public GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, OP op);
    public GSObject OperateGeneratorSequence(GeneratorSequence other, OP op);
}



