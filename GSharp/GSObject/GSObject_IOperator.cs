namespace GSharp.GSObject;

using System;
using GSharp.GSObject.Collections;
using GSharp.GSObject.Figures;

public abstract class GSObject: IOperable<GSObject, Add>, 
                                IOperable<GSObject, Subst>, 
                                IOperable<GSObject, Mult>, 
                                IOperable<GSObject, Div>, 
                                IOperable<GSObject, Mod>, 
                                IOperable<GSObject, LessTh>
{
    public abstract override string ToString();
    public abstract bool Equals(GSObject obj);
    public abstract string GetTypeName();
    public abstract bool GetTruthValue();

    protected static string NoOrderRelation(string T1, string T2)
        => $"No order relation between {T1} and {T2}";

    protected static string NoOrderRelation(string T1, GSTypes T2)
        => $"No order relation between {T1} and {T2}";

    protected static string TriedToOperate(string operation, string T1, string T2)
        => $"Tried to {operation} {T1} with {T2}";

    protected static string TriedToOperate(string operation, string T1, GSTypes T2)
        => $"Tried to {operation} {T1} with {T2}";

    protected static string TriedToFindModuloOfT1OverT2(string T1, string T2)
        => $"Tried to find Modulo of {T1} over {T2}";   

    protected static string TriedToFindModuloOfT1OverT2(string T1, GSTypes T2)
        => $"Tried to find Modulo of {T1} over {T2}";   

    public GSObject UnsupportedOperation(GSObject other, Operator OP)
    {
        if (OP is Add)
            throw new RuntimeError(TriedToOperate(ADD, this.GetTypeName(), other.GetTypeName()));
        if (OP is Subst)
            throw new RuntimeError(TriedToOperate(SUBSTRACT, this.GetTypeName(), other.GetTypeName()));
        if (OP is Mult)
            throw new RuntimeError(TriedToOperate(MULTIPLY, this.GetTypeName(), other.GetTypeName()));
        if (OP is Div)
            throw new RuntimeError(TriedToOperate(DIVIDE, this.GetTypeName(), other.GetTypeName()));
        if (OP is Mod)
            throw new RuntimeError(TriedToFindModuloOfT1OverT2(this.GetTypeName(), other.GetTypeName()));
        if (OP is LessTh)
            throw new RuntimeError(NoOrderRelation(this.GetTypeName(), other.GetTypeName()));

        throw new NotImplementedException("UNSUPPORTED OPERATOR");    
    }

    #region Operation Names

    protected const string ADD = "Add";
    protected const string SUBSTRACT = "Substract";
    protected const string MULTIPLY = "Multiply";
    protected const string DIVIDE = "Divide";

    #endregion

    #region Type Names



    // protected const string POINT = "Point";
    // protected const string LINE = "Line";
    // protected const string SEGMENT = "Segment";
    // protected const string RAY = "Ray";
    // protected const string CIRCLE = "Circle";
    // protected const string ARC = "Arc";
    
    // protected const string BOOLEAN = "Boolean";
    // protected const string STRING = "String";
    // protected const string SCALAR = "Scalar";

    protected const string SEQUENCE = "Sequence";
    protected const string UNDEFINED = "Undefined";
    
    #endregion

    #region OperatePoint

    public abstract GSObject OperatePoint(Point other, Add op);
    public abstract GSObject OperatePoint(Point other, Subst op);
    public abstract GSObject OperatePoint(Point other, Mult op);
    public GSObject OperatePoint(Point other, Div op) => UnsupportedOperation(other, op);
    public GSObject OperatePoint(Point other, Mod op) => UnsupportedOperation(other, op);
    public GSObject OperatePoint(Point other, LessTh op) => UnsupportedOperation(other, op);

    #endregion

    #region OperateLine

    public GSObject OperateLine(Line other, Add op) => UnsupportedOperation(other, op);
    public GSObject OperateLine(Line other, Subst op) => UnsupportedOperation(other, op);
    public GSObject OperateLine(Line other, Mult op) => UnsupportedOperation(other, op);
    public GSObject OperateLine(Line other, Div op) => UnsupportedOperation(other, op);
    public GSObject OperateLine(Line other, Mod op) => UnsupportedOperation(other, op);
    public GSObject OperateLine(Line other, LessTh op) => UnsupportedOperation(other, op);
    
    #endregion
   
    #region OperateRay
    public GSObject OperateRay(Ray other, Add op) => UnsupportedOperation(other, op);
    public GSObject OperateRay(Ray other, Subst op) => UnsupportedOperation(other, op);
    public GSObject OperateRay(Ray other, Mult op) => UnsupportedOperation(other, op);
    public GSObject OperateRay(Ray other, Div op) => UnsupportedOperation(other, op);
    public GSObject OperateRay(Ray other, Mod op) => UnsupportedOperation(other, op);
    public GSObject OperateRay(Ray other, LessTh op) => UnsupportedOperation(other, op);
    
    #endregion
    
    #region OperateSegment
    public GSObject OperateSegment(Segment other, Add op) => UnsupportedOperation(other, op);
    public GSObject OperateSegment(Segment other, Subst op) => UnsupportedOperation(other, op);
    public GSObject OperateSegment(Segment other, Mult op) => UnsupportedOperation(other, op);
    public GSObject OperateSegment(Segment other, Div op) => UnsupportedOperation(other, op);
    public GSObject OperateSegment(Segment other, Mod op) => UnsupportedOperation(other, op);
    public GSObject OperateSegment(Segment other, LessTh op) => UnsupportedOperation(other, op);
    
    #endregion
   
    #region OperateCircle
   
    public GSObject OperateCircle(Circle other, Add op) => UnsupportedOperation(other, op);
    public GSObject OperateCircle(Circle other, Subst op) => UnsupportedOperation(other, op);
    public GSObject OperateCircle(Circle other, Mult op) => UnsupportedOperation(other, op);
    public GSObject OperateCircle(Circle other, Div op) => UnsupportedOperation(other, op);
    public GSObject OperateCircle(Circle other, Mod op) => UnsupportedOperation(other, op);
    public GSObject OperateCircle(Circle other, LessTh op) => UnsupportedOperation(other, op);
    
    #endregion
    
    #region OperateArc

    public GSObject OperateArc(Arc other, Add op) => UnsupportedOperation(other, op);
    public GSObject OperateArc(Arc other, Subst op) => UnsupportedOperation(other, op);
    public GSObject OperateArc(Arc other, Mult op) => UnsupportedOperation(other, op);
    public GSObject OperateArc(Arc other, Div op) => UnsupportedOperation(other, op);
    public GSObject OperateArc(Arc other, Mod op) => UnsupportedOperation(other, op);
    public GSObject OperateArc(Arc other, LessTh op) => UnsupportedOperation(other, op);
   
    #endregion
   
    #region OperateScalar

    public abstract GSObject OperateScalar(Scalar other, Add op);
    public abstract GSObject OperateScalar(Scalar other, Subst op);
    public abstract GSObject OperateScalar(Scalar other, Mult op);
    public abstract GSObject OperateScalar(Scalar other, Div op);
    public abstract GSObject OperateScalar(Scalar other, Mod op);
    public abstract GSObject OperateScalar(Scalar other, LessTh op);

    #endregion

    #region OperateBoolean
    
    public GSObject OperateBoolean(Boolean other, Add op) => UnsupportedOperation(other, op);
    public GSObject OperateBoolean(Boolean other, Subst op) => UnsupportedOperation(other, op);
    public GSObject OperateBoolean(Boolean other, Mult op) => UnsupportedOperation(other, op);
    public GSObject OperateBoolean(Boolean other, Div op) => UnsupportedOperation(other, op);
    public GSObject OperateBoolean(Boolean other, Mod op) => UnsupportedOperation(other, op);
    public GSObject OperateBoolean(Boolean other, LessTh op) => UnsupportedOperation(other, op);

    #endregion

    #region OperateString

    public abstract GSObject OperateString(String other, Add op);
    public GSObject OperateString(String other, Subst op) => UnsupportedOperation(other, op);
    public GSObject OperateString(String other, Mult op) => UnsupportedOperation(other, op);
    public GSObject OperateString(String other, Div op) => UnsupportedOperation(other, op);
    public GSObject OperateString(String other, Mod op) => UnsupportedOperation(other, op);
    public GSObject OperateString(String other, LessTh op) => UnsupportedOperation(other, op);

    #endregion

    #region OperateUndefined

    public abstract GSObject OperateUndefined(Undefined other, Add op);
    public GSObject OperateUndefined(Undefined other, Subst op) => UnsupportedOperation(other, op);
    public GSObject OperateUndefined(Undefined other, Mult op) => UnsupportedOperation(other, op);
    public GSObject OperateUndefined(Undefined other, Div op) => UnsupportedOperation(other, op);
    public GSObject OperateUndefined(Undefined other, Mod op) => UnsupportedOperation(other, op);
    public GSObject OperateUndefined(Undefined other, LessTh op) => UnsupportedOperation(other, op);

    #endregion

    #region OperateFiniteStaticSequence
   
    public abstract GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Add op);
    public GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Subst op) => UnsupportedOperation(other, op);
    public GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Mult op) => UnsupportedOperation(other, op);
    public GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Div op) => UnsupportedOperation(other, op);
    public GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, Mod op) => UnsupportedOperation(other, op);
    public GSObject OperateFiniteStaticSequence(FiniteStaticSequence other, LessTh op) => UnsupportedOperation(other, op);

   
    #endregion

    #region OperateInfiniteStaticSequence

    public abstract GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Add op);
    public GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Subst op) => UnsupportedOperation(other, op);
    public GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Mult op) => UnsupportedOperation(other, op);
    public GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Div op) => UnsupportedOperation(other, op);
    public GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, Mod op) => UnsupportedOperation(other, op);
    public GSObject OperateInfiniteStaticSequence(InfiniteStaticSequence other, LessTh op) => UnsupportedOperation(other, op);
    
    #endregion
    
    #region OperateGeneratorSequence
    
    public abstract GSObject OperateGeneratorSequence(GeneratorSequence other, Add op);

    public GSObject OperateGeneratorSequence(GeneratorSequence other, Subst op) => UnsupportedOperation(other, op);
    public GSObject OperateGeneratorSequence(GeneratorSequence other, Mult op) => UnsupportedOperation(other, op);
    public GSObject OperateGeneratorSequence(GeneratorSequence other, Div op) => UnsupportedOperation(other, op);
    public GSObject OperateGeneratorSequence(GeneratorSequence other, Mod op) => UnsupportedOperation(other, op);
    public GSObject OperateGeneratorSequence(GeneratorSequence other, LessTh op) => UnsupportedOperation(other, op);

    #endregion
}

public interface IOperable<Return, OP> where OP: Operator
{
    public static Return Operate(IOperable<Return, OP> operable1, IOperable<Return, OP> operable2)
    {
        if (operable2 is Point P) return operable1.OperatePoint(P, null);
        if (operable2 is Line L) return operable1.OperateLine(L, null);
        if (operable2 is Segment S) return operable1.OperateSegment(S, null);
        if (operable2 is Circle C) return operable1.OperateCircle(C, null);
        if (operable2 is Arc A) return operable1.OperateArc(A, null);
        
        
        if (operable2 is Boolean B) return operable1.OperateBoolean(B, null);
        if (operable2 is Scalar scalar) return operable1.OperateScalar(scalar, null);
        if (operable2 is String str) return operable1.OperateString(str, null);
        if (operable2 is Undefined u) return operable1.OperateUndefined(u, null);

        throw new NotImplementedException("GSOBJECT UNSUPPORTED");
    }

    public Return OperatePoint(Point other, OP op);
    public Return OperateLine(Line other, OP op);
    public Return OperateSegment(Segment other, OP op);
    public Return OperateRay(Ray other, OP op);
    public Return OperateCircle(Circle other, OP op);
    public Return OperateArc(Arc other, OP op);
    public Return OperateScalar(Scalar other, OP op);
    public Return OperateBoolean(Boolean other, OP op);
    public Return OperateString(String other, OP op);
    public Return OperateUndefined(Undefined other, OP op);
    public Return OperateFiniteStaticSequence(FiniteStaticSequence other, OP op);
    public Return OperateInfiniteStaticSequence(InfiniteStaticSequence other, OP op);
    public Return OperateGeneratorSequence(GeneratorSequence other, OP op);
}

public abstract class Operator {}

public abstract class Add : Operator {}

public abstract class Subst : Operator {}

public abstract class Mult : Operator {}

public abstract class Div : Operator {}

public abstract class Mod : Operator {}

public abstract class LessTh : Operator {}