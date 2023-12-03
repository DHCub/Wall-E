namespace GSharp.SemanticAnalyzer;
using GSharp.Expression;
using GSharp.Statement;
using GSharp.Types;
using System.Collections.Generic;

public class SemanticAnalyzer : Stmt.IVisitor<GSType>, Expr.IVisitor<GSType>
{
    private Context BuiltIns;
    private Context CurrentContext;

    public SemanticAnalyzer()
    {
		void DefineBuiltIns()
		{
			const string POINT = "point";
			const string LINE = "line";
			const string RAY = "ray";
			const string SEGMENT = "segment";
			const string CIRCLE = "circle";
			const string ARC = "arc";
			const string MEASURE = "measure";
			const string INTERSECT = "intersect";
			const string COUNT = "count";
			const string RANDOMS = "randoms";
			const string POINTS = "points"; // sample figure
			const string SAMPLES = "samples";

			BuiltIns.Define(POINT, new Fun_Symbol(
				POINT,
				new List<(GSType, string)>{
					(new SimpleType(TypeName.Scalar), "x"),
					(new SimpleType(TypeName.Scalar), "y")
				},
				new SimpleType(TypeName.Point)
			), 2);

			BuiltIns.Define(LINE, new Fun_Symbol(
				LINE,
				new List<(GSType, string)>{
					(new SimpleType(TypeName.Point), "p1"),
					(new SimpleType(TypeName.Point), "p2")
				},
				new SimpleType(TypeName.Line)
			), 2);

			BuiltIns.Define(RAY, new Fun_Symbol(
				RAY,
				new List<(GSType, string)>{
					(new SimpleType(TypeName.Point), "p1"),
					(new SimpleType(TypeName.Point), "p2")
				},
				new SimpleType(TypeName.Ray)
			), 2);

			BuiltIns.Define(SEGMENT, new Fun_Symbol(
				SEGMENT,
				new List<(GSType, string)>{
					(new SimpleType(TypeName.Point), "p1"),
					(new SimpleType(TypeName.Point), "p2")
				},
				new SimpleType(TypeName.Segment)
			), 2);

			BuiltIns.Define(CIRCLE, new Fun_Symbol(
				CIRCLE,
				new List<(GSType, string)>{
					(new SimpleType(TypeName.Point), "c"),
					(new SimpleType(TypeName.Measure), "r")
				},
				new SimpleType(TypeName.Circle)
			), 2);

			BuiltIns.Define(ARC, new Fun_Symbol(
				ARC,
				new List<(GSType, string)>{
					(new SimpleType(TypeName.Point), "p1"),
					(new SimpleType(TypeName.Point), "p2"),
					(new SimpleType(TypeName.Point), "p3"),
					(new SimpleType(TypeName.Measure), "m")
				},
				new SimpleType(TypeName.Arc)
			), 4);

			BuiltIns.Define(MEASURE, new Fun_Symbol(
				MEASURE,
				new List<(GSType, string)>{
					(new SimpleType(TypeName.Point), "p1"),
					(new SimpleType(TypeName.Point), "p2")
				},
				new SimpleType(TypeName.Measure)
			), 2);

			BuiltIns.Define(INTERSECT, new Fun_Symbol(
				INTERSECT,
				new List<(GSType, string)>{
					(new DrawableType(), "f1"),
					(new DrawableType(), "f2")
				},
				new SequenceType(new SimpleType(TypeName.Point))
			), 2);


			BuiltIns.Define(COUNT, new Fun_Symbol(
				COUNT,
				new List<(GSType, string)>{
					(new SequenceType(new UndefinedType()), "s")
				},
				new SimpleType(TypeName.Scalar)
			), 1);

			BuiltIns.Define(RANDOMS, new Fun_Symbol(
				RANDOMS,
				new(),
				new SequenceType(new SimpleType(TypeName.Scalar))
			), 0);

			BuiltIns.Define(POINTS, new Fun_Symbol(
				POINTS,
				new List<(GSType, string)>{
					(new DrawableType(), "f")
				},
				new SequenceType(new SimpleType(TypeName.Point))
			), 1);

			BuiltIns.Define(SAMPLES, new Fun_Symbol(
				SAMPLES,
				new(),
				new SequenceType(new SimpleType(TypeName.Point))
			), 0);

		}

        BuiltIns = new();

    }

    public GSType TypeCheck(Stmt stmt) => stmt.Accept(this);
    public GSType TypeCheck(Expr expr) => expr.Accept(this);
}