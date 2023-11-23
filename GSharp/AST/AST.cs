using GSharp;

using System.Text;

public class ASTPrinter : Expr.IVisitor<string>, Stmt.IVisitor<string>
{
  public string Print(Expr expr)
  {
    return expr.Accept(this);
  }

  public string Print(Stmt stmt)
  {
    return stmt.Accept(this);
  }

  public string VisitColorStmt(Color stmt)
  {
    return "(color " + stmt.color + ")";
  }

  public string VisitConstantStmt(Constant stmt)
  {
    string result = "(constant ["; bool addComma = false;
    foreach (var item in stmt.constNames)
    {
      if (addComma) result += ", ";
      result += item.lexeme;
      addComma = true;
    }
    result += "]";
    result += " = ";

    result += Print(stmt.initializer);
    result += ")";
    return result;
  }

  public string VisitDrawStmt(Draw stmt)
  {
    string result = "(draw " + Print(stmt.elements);
    if (!(stmt.nameTk is null)) result += "=> " + stmt.nameTk.lexeme;
    result += ")";
    return result;
  }

  public string VisitExpressionStmt(Expression stmt)
  {
    return "(expression" + Print(stmt.expression) + ")";
  }

  public string VisitFunctionStmt(Function stmt)
  {
    string result = "(function " + stmt.name.lexeme + "[";
    bool addComma = false;
    foreach (var item in stmt.parameters)
    {
      if (addComma) result += ", ";
      result += item.lexeme;
      addComma = true;
    }
    result += "]";
    result += " = ";
    result += Print(stmt.body);
    result += ")";
    return result;
  }

  public string VisitImportStmt(Import stmt)
  {
    return "(import " + stmt.dirName.lexeme + ")";
  }

  public string VisitPrintStmt(Print stmt)
  {
    string result = "(print ["; bool addComma = false;
    foreach (var item in stmt.printe)
    {
      if (addComma) result += ", ";
      result += Print(item);
    }
    result += "]";
    return result;
  }

  public string VisitRestoreStmt(Restore stmt)
  {
    return "(restore)";
  }

  public string VisitVarStmt(Var stmt)
  {
    string result = "(var " + stmt.type.type + " " + stmt.name.lexeme;
    result += "[" + stmt.isSequence + "]";
    if (!(stmt.initializer is null)) result += "= " + Print(stmt.initializer);
    result += ")";
    return result;
  }

  public string VisitAssignExpr(Assign expr)
  {
    return Parenthesize2("=", expr.name, expr.value);
  }

  public string VisitBinaryExpr(Binary expr)
  {
    return Parenthesize(expr.oper.lexeme, expr.left, expr.right);
  }

  public string VisitCallExpr(Call expr)
  {
    return Parenthesize2("call", expr.calle, expr.parameters);
  }

  public string VisitConditionalExpr(Conditional expr)
  {
    return Parenthesize2("if-else", expr.condition, expr.thenBranch, expr.elseBranch);
  }

  public string VisitLetInExpr(LetIn expr)
  {
    return Parenthesize2("let-in", expr.instructions, expr.body);
  }

  public string VisitLiteralExpr(Literal expr)
  {
    if (expr.value == null) return "null";
    return expr.value.ToString();
  }

  public string VisitLogicalExpr(Logical expr)
  {
    return Parenthesize(expr.oper.lexeme, expr.left, expr.right);
  }

  public string VisitRangeExpr(GSharp.Range expr)
  {
    if (expr.right == null)
    {
      return "range [" + expr.left.lexeme + ", oo)";
    }

    return "range [" + expr.left.lexeme + ", " + expr.right.lexeme + "]";
  }

  public string VisitSequenceExpr(Sequence expr)
  {
    return Parenthesize2("sequence", expr.items);
  }

  public string VisitUnaryExpr(Unary expr)
  {
    return Parenthesize(expr.oper.lexeme, expr.right);
  }

  public string VisitUndefinedExpr(Undefined expr)
  {
    return "undefined";
  }

  public string VisitVariableExpr(Variable expr)
  {
    return expr.name.lexeme;
  }

  private string Parenthesize(string name, params Expr[] exprs)
  {
    var result = new StringBuilder();
    result.Append("(").Append(name);
    foreach (var expr in exprs)
    {
      result.Append(" ");
      result.Append(expr.Accept(this));
    }
    result.Append(")");
    return result.ToString();
  }

  private string Parenthesize2(string name, params object[] parts)
  {
    var result = new StringBuilder();
    result.Append("(").Append(name);

    foreach (var part in parts)
    {
      result.Append(" ");

      switch (part)
      {
        case Expr expr:
          result.Append(expr.Accept(this));
          break;
        case Token token:
          result.Append(token.lexeme);
          break;
        case IEnumerable<Expr> expressions:
          if (expressions.Any())
          {
            result.Append("[");
            foreach (var expr in expressions)
            {
              if (expr != expressions.First())
              {
                result.Append(", ");
              }
              result.Append(expr.Accept(this));
            }
            result.Append("]");
          }
          break;
        default:
          result.Append(part.ToString());
          break;
      }
    }

    result.Append(")");
    return result.ToString();
  }
}