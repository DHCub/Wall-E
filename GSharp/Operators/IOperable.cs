namespace GSharp;

using System;
using System.Drawing;
using GSharp.Types;

public interface IOperable<OP> where OP: Operator
{
    public static (GSType retType, string? errorMessage) Operable(IOperable<OP> A, IOperable<OP> B)
    {
        OP op = null;
        if (B is SimpleType simpleType)
        {
            return simpleType.type switch{
                TypeName.Point => A.OperablePoint(op),
                TypeName.Line => A.OperableLine(op),
                TypeName.Segment => A.OperableSegment(op),
                TypeName.Ray => A.OperableRay(op),
                TypeName.Circle => A.OperableCircle(op),
                TypeName.Arc => A.OperableArc(op),
                TypeName.Scalar => A.OperableScalar(op),
                TypeName.Measure => A.OperableMeasure(op),
                TypeName.String => A.OperableString(op),

                _ => throw new Exception("UNSUPPORTED SIMPLETYPE")
            };
        }
        if (B is SequenceType seq)
            return A.OperableSequence(seq, op);
        
        if (B is DrawableType) return A.OperableDrawable(op);
        if (B is FigureType) return A.OperableFigure(op);
        if (B is UndefinedType) return A.OperableUndefined(op);

        throw new Exception("UNSUPPORTED GSTYPE");
    }

    (GSType retType, string errorMessage) UnsupportedOperator(string otherT, OP op);

    (GSType retType, string? errorMessage) OperablePoint(OP op);
    (GSType retType, string? errorMessage) OperableLine(OP op);
    (GSType retType, string? errorMessage) OperableSegment(OP op);
    (GSType retType, string? errorMessage) OperableRay(OP op);
    (GSType retType, string? errorMessage) OperableCircle(OP op);
    (GSType retType, string? errorMessage) OperableArc(OP op);
    
    (GSType retType, string? errorMessage) OperableScalar(OP op);
    (GSType retType, string? errorMessage) OperableMeasure(OP op);
    (GSType retType, string? errorMessage) OperableString(OP op);

    (GSType retType, string? errorMessage) OperableSequence(SequenceType other, OP op);
    (GSType retType, string? errorMessage) OperableUndefined(OP op);
    
    (GSType retType, string? errorMessage) OperableDrawable(OP op);
    (GSType retType, string? errorMessage) OperableFigure(OP op);

}