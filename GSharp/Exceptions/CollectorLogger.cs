namespace GSharp;

using System.Collections.Generic;
using Godot;

class CollectorLogger : ILogger
{
    public List<string> Errors = new();

    public bool hadError { get; private set; }

    public bool hadRuntimeError { get; private set; }

    public void Error(string type, int line, int column, string where, string message)
    {
        Errors.Add($"! {type} ERROR [{line}:{column}] {where}: {message}");
        hadError = true;
    }

    public void RuntimeError(RuntimeError error)
    {
        Errors.Add($"! SEMANTIC ERROR: `{error.tokenStr}` {error.Message}");
        hadRuntimeError = true;
    }

    public void ResetError()
    {
        hadError = false;
    }

    public void ResetRuntimeError()
    {
        hadRuntimeError = false;
    }
}