namespace GSharp;

public abstract class Stmt
{
  public interface IVisitor<R>
  {
    R VisitColorStmt(Color stmt);
    R VisitConstantStmt(Constant stmt);
    R VisitDrawStmt(Draw stmt);
    R VisitExpressionStmt(Expression stmt);
    R VisitFunctionStmt(Function stmt);
    R VisitImportStmt(Import stmt);
    R VisitRestoreStmt(Restore stmt);
    R VisitVarStmt(Var stmt);
  }

  public abstract R Accept<R>(IVisitor<R> visitor);
}