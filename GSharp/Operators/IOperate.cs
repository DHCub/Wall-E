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
        if (operable2 is Ray R) return operable1.OperateRay(R, null);
        if (operable2 is Circle C) return operable1.OperateCircle(C, null);
        if (operable2 is Arc A) return operable1.OperateArc(A, null);
        
        
        if (operable2 is Scalar scalar) return operable1.OperateScalar(scalar, null);
        if (operable2 is Measure measure) return operable1.OperateMeasure(measure, null);
        if (operable2 is Objects.String str) return operable1.OperateString(str, null);
        if (operable2 is Undefined u) return operable1.OperateUndefined(u, null);

        if (operable2 is FiniteStaticSequence finSeq) return operable1.OperateFiniteStaticSequence(finSeq, null);
        if (operable2 is InfiniteStaticSequence infSeq) return operable1.OperateInfiniteStaticSequence(infSeq, null);
        if (operable2 is GeneratorSequence genSeq) return operable1.OperateGeneratorSequence(genSeq, null);

        throw new NotImplementedException("GSOBJECT UNSUPPORTED");
    }

    GSObject UnsupportedOperError(Objects.GSObject other, OP op);

    GSObject OperatePoint(Point other, OP op);
    GSObject OperateLine(Line other, OP op);
    GSObject OperateSegment(Segment other, OP op);
    GSObject OperateRay(Ray other, OP op);
    GSObject OperateCircle(Circle other, OP op);
    GSObject OperateArc(Arc other, OP op);
    
    
    GSObject OperateScalar(Scalar other, OP op);
    GSObject OperateMeasure(Measure other, OP op);
    
    
    GSObject OperateString(Objects.String other, OP op);
    
    
    GSObject OperateUndefined(Undefined other, OP op);
    
    
    GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, OP op);
    GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, OP op);
    GSObject OperateGeneratorSequence(GeneratorSequence other, OP op);
}



