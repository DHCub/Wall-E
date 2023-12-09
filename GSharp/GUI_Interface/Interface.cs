using System;
using System.Collections.Generic;
using GSharp.Exceptions;
using GSharp.Objects.Figures;
using GSharp.Parser;
using GSharp.Statement;

namespace GSharp.GUIInterface;


public enum Colors
{
  Black,
  Red,
  Blue,
  Green,
  Purple,
  Cyan,
  Magenta,
  Yellow,
  White,
  Gray,
}
public class GUIInterface
{
  public static Colors GetColor(string color) => color switch
  {
    "black" => Colors.Black,
    "red" => Colors.Red,
    "blue" => Colors.Blue,
    "green" => Colors.Green,
    "purple" => Colors.Purple,
    "cyan" => Colors.Cyan,
    "magenta" => Colors.Magenta,
    "yellow" => Colors.Yellow,
    "white" => Colors.White,
    "gray" => Colors.Gray,

    _ => throw new NotImplementedException("UNSUPPORTED COLOR IN GUI INTERFACE")
  };

  Action<Colors, Figure> drawFigure;
  Action<Colors, Figure, string> drawLabeledFigure;
  Action<string> standardOutputHandler;
  Action<string> errorHandler;
  Func<string, string> importHandler;

  public GUIInterface(Action<string> standardOutputHandler, Action<string> errorHandler, Func<string, string> importHandler, Action<Colors, Figure> drawFigure, Action<Colors, Figure, string> drawLabeledFigure)
  {
    this.drawFigure = drawFigure;
    this.drawLabeledFigure = drawLabeledFigure;
    this.standardOutputHandler = standardOutputHandler;
    this.errorHandler = errorHandler;
    this.importHandler = importHandler;
  }

  // public void Interpret(string source)
  // {
  //   bool hadError = false;
  //   void ScanErrorHandler(ScanError error)
  //   {
  //     errorHandler($"! SCAN ERROR : at {error.line}: " + error.Message);
  //     hadError = true;
  //   }
  //   void ParseErrorHandler(ParseError error)
  //   {
  //     errorHandler(error.ToString());
  //     hadError = true;
  //   }
  //   void runtimeErrorHandler(RuntimeError error)
  //   {
  //     errorHandler(error.ToString());
  //     hadError = true;
  //   }
  //   List<Stmt> importHandler(string dir) => throw new NotImplementedException();

  //   var interpreter = new Interpreter.Interpreter(runtimeErrorHandler, standardOutputHandler, importHandler, drawFigure, drawLabeledFigure);

  //   interpreter.Eval(source, ScanErrorHandler, ParseErrorHandler, )

  // }

  public void Interpret(string source)
  {
    void runtimeErrorHandler(RuntimeError error)
    {
      errorHandler(error.ToString());
    }

    void ScanError(ScanError scanError)
    {
      // ReportError(scanError.line, string.Empty, scanError.Message);
      errorHandler(scanError.ToString());
    }

    void RuntimeError(RuntimeError error)
    {
      errorHandler(error.ToString());
    }

    void ParseError(ParseError parseError)
    {
      // Error(parseError.token, parseError.Message);
      errorHandler(parseError.ToString());
    }

    void NameResolutionError(NameResolutionError nameResolutionError)
    {
      // Error(nameResolutionError.Token, nameResolutionError.Message);
      errorHandler(nameResolutionError.ToString());
    }

    void SemanticAnalyzerError(SemanticError semanticError)
    {
      errorHandler(semanticError.ToString());
    }

    var interpreter = new Interpreter.Interpreter(runtimeErrorHandler, standardOutputHandler, importHandler, drawFigure, drawLabeledFigure);
    object? result = interpreter.Eval(source, ScanError, ParseError, NameResolutionError, SemanticAnalyzerError);
  }
}