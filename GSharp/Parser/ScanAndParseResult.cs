using GSharp.Statement;
using GSharp.Expression;
using System.Collections.Generic;

namespace GSharp;

public class ScanAndParseResult
{

  public static ScanAndParseResult scanErrorOccurred { get; } = new();
  public static ScanAndParseResult parseErrorEncountered { get; } = new();

  public List<Stmt>? statements { get; }

  public bool hasStatements => statements != null;

  private ScanAndParseResult() { }

  private ScanAndParseResult(List<Stmt> statements)
  {
    this.statements = statements;
  }

  public static ScanAndParseResult OfStmts(List<Stmt> stmts)
  {
    return new ScanAndParseResult(stmts);
  }
}