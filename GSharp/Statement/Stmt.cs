namespace GSharp.Statement;

public abstract class Stmt
{
  public interface IVisitor<R>
  {
    R VisitBlockStmt(Block stmt);
    R VisitColorStmt(ColorStmt stmt);
    R VisitConstantStmt(ConstantStmt stmt);
    R VisitDrawStmt(Draw stmt);
    R VisitExpressionStmt(ExpressionStmt stmt);
    R VisitFunctionStmt(Function stmt);
    R VisitImportStmt(Import stmt);
    R VisitPrintStmt(Print stmt);
    R VisitRestoreStmt(Restore stmt);
    R VisitReturnStmt(Return stmt);
    R VisitVarStmt(Var stmt);
    R VisitWhileStmt(While stmt);
  }

  public abstract R Accept<R>(IVisitor<R> visitor);
}