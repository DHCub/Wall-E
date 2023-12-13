#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GSharp.Exceptions;
using GSharp.Expression;
using GSharp.Statement;

namespace GSharp.Interpreter;

/// <summary>
/// The NameResolver is responsible for resolving names of local and global variable/function names.
/// </summary>
internal class NameResolver : Expr.IVisitor<VoidObject>, Stmt.IVisitor<VoidObject>
{
  private readonly IBindingHandler bindingHandler;
  private readonly NameResolutionErrorHandler nameResolutionErrorHandler;

  // an instance-local list of scopes
  private readonly List<IDictionary<string, IBindingFactory>> scopes = new List<IDictionary<string, IBindingFactory>>();

  // an instance-local list of global symbols (variables, functions)
  private readonly IDictionary<string, IBindingFactory> globals = new Dictionary<string, IBindingFactory>();

  internal IDictionary<string, IBindingFactory> Globals => globals;

  private FunctionType currentFunction = FunctionType.NONE;

  private HashSet<string> builtins = new HashSet<string>()
  {
    "line",
    "segment",
    "ray",
    "arc",
    "circle",
    "measure",
    "intersect",
    "count",
    "randoms",
    "points",
    "samples",
    "point"
  };

  internal NameResolver(IBindingHandler bindingHandler, NameResolutionErrorHandler nameResolutionErrorHandler)
  {
    this.bindingHandler = bindingHandler;
    this.nameResolutionErrorHandler = nameResolutionErrorHandler;
  }

  internal void Resolve(IEnumerable<Stmt> statements)
  {
    foreach (var statement in statements)
    {
      Resolve(statement);
    }
  }

  internal void Resolve(Expr expr)
  {
    expr.Accept(this);
  }

  private void BeginScope()
  {
    scopes.Add(new Dictionary<string, IBindingFactory>());
  }

  private void EndScope()
  {
    scopes.RemoveAt(scopes.Count - 1);
  }

  // declares a variable or function as existing (but not yet initialized) in the
  // innermost scope. This allows the variable to shadow variables in outer scopes
  // with the same name.
  private void Declare(Token name)
  {
    if (name.lexeme == "_") return;

    if (IsEmpty(scopes))
    {
      return;
    }

    // this adds the variable to the innermost scope so that it shadows any outer one
    // and so that we know the variable exists.
    var scope = scopes.Last();

    if (builtins.Contains(name.lexeme))
    {
      nameResolutionErrorHandler(new NameResolutionError("Native function with this name is available.", name));
      return;
    }

    if (scope.ContainsKey(name.lexeme))
    {
      nameResolutionErrorHandler(new NameResolutionError("Variable with this name already declared in this scope.", name));
      return;
    }

    // we mark it as "not ready yet" by binding a know None-value in the scope map. Each
    // value in the scope map means "is finished being initialized", at this stage of
    // traversing the tree.
    scope[name.lexeme] = VariableBindingFactory.None;
  }

  private static bool IsEmpty(ICollection stack)
  {
    return stack.Count == 0;
  }

  // defines a previously declared variable as initialized, available for use.
  private void Define(Token name, ITypeReference typeReference)
  {
    if (name.lexeme == "_") return;

    if (builtins.Contains(name.lexeme))
    {
      nameResolutionErrorHandler(new NameResolutionError("Native function with this name is available.", name));
      return;
    }

    if (typeReference == null)
    {
      throw new ArgumentException("typeReference cannot be null");
    }

    if (IsEmpty(scopes))
    {
      globals[name.lexeme] = new VariableBindingFactory(typeReference);
      return;
    }

    // we set the variable's value in the scope map to mark it as fully initialized
    // and available for use. It's alive!
    scopes.Last()[name.lexeme] = new VariableBindingFactory(typeReference);
  }

  private void DefineFunction(Token name, ITypeReference typeReference, Function function)
  {
    if (builtins.Contains(name.lexeme))
    {
      nameResolutionErrorHandler(new NameResolutionError("Native function with this name is available.", name));
      return;
    }

    if (typeReference == null)
    {
      throw new ArgumentException("typeReference cannot be null");
    }

    if (IsEmpty(scopes))
    {
      globals[name.lexeme] = new FunctionBindingFactory(typeReference, function);
      return;
    }

    scopes.Last()[name.lexeme] = new FunctionBindingFactory(typeReference, function);
  }

  private void ResolveLocalOrGlobal(Expr referringExpr, Token name)
  {
    // loop over all the scopes, from the innermost and outhards, trying to find a registered
    // "binding factory" that matches this name.
    for (int i = scopes.Count - 1; i >= 0; i--)
    {
      if (scopes[i].ContainsKey(name.lexeme))
      {
        IBindingFactory bindingFactory = scopes[i][name.lexeme];

        if (bindingFactory == VariableBindingFactory.None)
        {
          nameResolutionErrorHandler(new NameResolutionError("Cannot read local variable in its own initializer.", name));
          return;
        }

        bindingHandler.AddLocalExpr(bindingFactory.CreateBinding(scopes.Count - 1 - i, referringExpr));

        return;
      }
    }

    // the identifier was not found in any of the local scopes. if it cannot be found in the globals
    // we can safely assume it is non-existent

    if (!globals.ContainsKey(name.lexeme))
    {
      return;
    }

    {
      IBindingFactory bindingFactory = globals[name.lexeme];
      bindingHandler.AddGlobalExpr(bindingFactory.CreateBinding(-1, referringExpr));
    }
  }

  public VoidObject VisitAssignExpr(Assign expr)
  {
    Resolve(expr.Value);
    ResolveLocalOrGlobal(expr, expr.Name);

    return VoidObject.Void;
  }

  public VoidObject VisitEmptyExpr(Empty expr)
  {
    return VoidObject.Void;
  }

  public VoidObject VisitBinaryExpr(Binary expr)
  {
    Resolve(expr.Left);
    Resolve(expr.Right);

    return VoidObject.Void;
  }

  public VoidObject VisitCallExpr(Call expr)
  {
    Resolve(expr.Calle);

    foreach (var argument in expr.Arguments)
    {
      Resolve(argument);
    }

    if (expr.Calle is Variable variableExpr)
    {
      ResolveLocalOrGlobal(expr, variableExpr.Name);
    }

    return VoidObject.Void;
  }

  public VoidObject VisitIndexExpr(Index expr)
  {
    Resolve(expr.Indexee);
    Resolve(expr.Argument);

    if (expr.Indexee is Variable variableExpr)
    {
      ResolveLocalOrGlobal(expr, variableExpr.Name);
    }

    return VoidObject.Void;
  }

  public VoidObject VisitGroupingExpr(Grouping expr)
  {
    Resolve(expr.Expression);

    return VoidObject.Void;
  }

  public VoidObject VisitLiteralExpr(Literal expr)
  {
    return VoidObject.Void;
  }

  public VoidObject VisitLogicalExpr(Logical expr)
  {
    Resolve(expr.Left);
    Resolve(expr.Right);

    return VoidObject.Void;
  }

  public VoidObject VisitUnaryPrefixExpr(UnaryPrefix expr)
  {
    Resolve(expr.Right);

    return VoidObject.Void;
  }

  public VoidObject VisitUnaryPostfixExpr(UnaryPostfix expr)
  {
    Resolve(expr.Left);
    ResolveLocalOrGlobal(expr, expr.Name);

    return VoidObject.Void;
  }

  public VoidObject VisitVariableExpr(Variable expr)
  {
    if (!IsEmpty(scopes) && scopes.Last().ContainsKey(expr.Name.lexeme))
    {
      if (scopes.Last()[expr.Name.lexeme] == VariableBindingFactory.None)
        nameResolutionErrorHandler(new NameResolutionError("Cannot read local variable in its own initializer.", expr.Name));
    }

    ResolveLocalOrGlobal(expr, expr.Name);

    return VoidObject.Void;
  }

  public VoidObject VisitBlockStmt(Block stmt)
  {
    BeginScope();
    Resolve(stmt.Statements);
    EndScope();

    return VoidObject.Void;
  }

  public VoidObject ExecuteBlock(List<Stmt> stmts)
  {
    BeginScope();

    foreach (var stmt in stmts)
    {
      Resolve(stmt);
    }

    EndScope();

    return VoidObject.Void;
  }

  private void Resolve(Stmt stmt)
  {
    stmt.Accept(this);
  }

  public VoidObject VisitConditionalExpr(Conditional expr)
  {
    Resolve(expr.Condition);
    Resolve(expr.ThenBranch);
    Resolve(expr.ElseBranch);

    return VoidObject.Void;
  }

  public VoidObject VisitIntRangeExpr(IntRange expr)
  {
    return VoidObject.Void;
  }

  public VoidObject VisitLetInExpr(LetIn expr)
  {
    ResolveLetIn(expr, FunctionType.LETIN);

    return VoidObject.Void;
  }

  public VoidObject VisitSequenceExpr(Sequence expr)
  {
    foreach (var items in expr.Items)
    {
      Resolve(items);
    }

    return VoidObject.Void;
  }

  public VoidObject VisitUndefinedExpr(Undefined expr)
  {
    return VoidObject.Void;
  }

  public VoidObject VisitColorStmt(ColorStmt stmt)
  {
    return VoidObject.Void;
  }

  public VoidObject VisitConstantStmt(ConstantStmt stmt)
  {
    foreach (var consts in stmt.Names)
    {
      if ((string)consts.literal != "_")
        Declare(consts);
    }

    Resolve(stmt.Initializer);

    foreach (var consts in stmt.Names)
    {
      if ((string)consts.literal != "_")
        Define(consts, new TypeReference(consts));
    }

    return VoidObject.Void;
  }

  public VoidObject VisitDrawStmt(Draw stmt)
  {
    Resolve(stmt.Elements);

    return VoidObject.Void;
  }

  public VoidObject VisitExpressionStmt(ExpressionStmt stmt)
  {
    Resolve(stmt.Expression);

    return VoidObject.Void;
  }

  public VoidObject VisitFunctionStmt(Function stmt)
  {
    Declare(stmt.Name);
    DefineFunction(stmt.Name, stmt.ReturnTypeReference, stmt);

    ResolveFunction(stmt, FunctionType.FUNCTION);
    return VoidObject.Void;
  }

  private void ResolveFunction(Function function, FunctionType type)
  {
    FunctionType enclosingFunction = currentFunction;
    currentFunction = type;

    BeginScope();

    foreach (var param in function.Parameters)
    {
      Declare(param.Name);
      Define(param.Name, new TypeReference(param.TypeSpecifier));
    }

    Resolve(function.Body);

    EndScope();

    currentFunction = enclosingFunction;
  }

  private void ResolveLetIn(LetIn letIn, FunctionType type)
  {
    FunctionType enclosingFunction = currentFunction;
    currentFunction = type;

    BeginScope();

    Resolve(letIn.Stmts);

    EndScope();

    currentFunction = enclosingFunction;
  }

  public VoidObject VisitImportStmt(Import stmt)
  {
    // in future check if the code to import is in a valid path
    return VoidObject.Void;
  }

  public VoidObject VisitPrintStmt(Print stmt)
  {
    Resolve(stmt.Expression);

    return VoidObject.Void;
  }

  public VoidObject VisitRestoreStmt(Restore stmt)
  {
    return VoidObject.Void;
  }

  public VoidObject VisitReturnStmt(Statement.Return stmt)
  {
    if (currentFunction == FunctionType.NONE)
    {
      nameResolutionErrorHandler(new NameResolutionError("Cannot return from top-level code.", stmt.Keyword));
    }

    if (stmt.Value != null)
    {
      Resolve(stmt.Value);
    }

    return VoidObject.Void;
  }

  public VoidObject VisitVarStmt(Var stmt)
  {
    Declare(stmt.Name);

    if (stmt.Initializer != null)
    {
      Resolve(stmt.Initializer);
    }

    Define(stmt.Name, stmt.TypeReference ?? new TypeReference(stmt.Name));

    return VoidObject.Void;
  }

  public VoidObject VisitWhileStmt(While stmt)
  {
    return VoidObject.Void;
    // Resolve(stmt.Condition);
    // Resolve(stmt.Body);

  }

  private enum FunctionType
  {
    NONE,
    FUNCTION,
    LETIN,
  }
}