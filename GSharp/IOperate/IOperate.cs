namespace GSharp;
using System;
using Objects;
using Objects.Figures;
using Objects.Collections;
public interface IOperate<Return, OP> where OP: Operator
{
    public static Return Operate(IOperate<Return, OP> operable1, IOperate<Return, OP> operable2)
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

    public Return UnsupportedOperError(Objects.GSObject other, OP op);

    public Return OperatePoint(Point other, OP op);
    public Return OperateLine(Line other, OP op);
    public Return OperateSegment(Segment other, OP op);
    public Return OperateRay(Ray other, OP op);
    public Return OperateCircle(Circle other, OP op);
    public Return OperateArc(Arc other, OP op);
    
    
    public Return OperateScalar(Scalar other, OP op);
    public Return OperateMeasure(Measure other, OP op);
    
    
    public Return OperateString(Objects.String other, OP op);
    
    
    public Return OperateUndefined(Undefined other, OP op);
    
    
    public Return OperateFiniteStaticSequence(FiniteStaticSequence other, OP op);
    public Return OperateInfiniteStaticSequence(InfiniteStaticSequence other, OP op);
    public Return OperateGeneratorSequence(GeneratorSequence other, OP op);
}



