using Godot;
using GSharp;
using GSharp.Exceptions;
using GSharp.Objects.Collections;
using GSharp.Interpreter;
using GSharp.Objects.Figures;
using System.Collections.Generic;
using System;

public partial class Control : Godot.Control
{
	static readonly Segment S = new(new(0, 0), new(200, 200));
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	public void _on_Button_pressed()
	{
		var code = GetNode<CodeEdit>("Code_Edit_Marg/CodeEdit");
		var draw_area = GetNode<Node2D>("Draw_Area_Marg/Viewport_Container/SubViewport/Background/Node2D");
		var console = GetNode<RichTextLabel>("Console_Margin/Console");
		console.Text = "";

		void ShowIntersect(params Figure[] arr)
		{
			for (int i = 0; i < arr.Length - 1; i++)
			{
				for (int j = i + 1; j < arr.Length; j++)
				{
					if (i == j) continue;
					var intersect = Functions.Intersect(arr[i], arr[j]);

					if (intersect is FiniteStaticSequence Seq)
					{
						foreach (var P in Seq.GetPrefixValues())
							draw_area.AddDrawable(Colors.Black, (Point)P);
					}
				}
			}
		}

		var txt = code.Text;

		bool hadError = false, hadRuntimeError = false;

		void ScanError(ScanError scanError)
		{
			ReportError(scanError.line, string.Empty, scanError.Message);
		}

		void RuntimeError(RuntimeError error)
		{
			string line = error.token?.line.ToString() ?? "unknown";

			console.Text += $"[line {line}] {error.Message}\n";
			hadRuntimeError = true;
		}

		void Error(Token token, string message)
		{
			if (token.type == TokenType.EOF)
			{
				ReportError(token.line, " at end", message);
			}
			else
			{
				ReportError(token.line, " at '" + token.lexeme + "'", message);
			}
		}

		void ReportError(int line, string where, string message)
		{
			Console.WriteLine($"[line {line}] Error{where}: {message}");
			hadError = true;
		}

		void ParseError(ParseError parseError)
		{
			Error(parseError.token, parseError.Message);
		}

		void NameResolutionError(NameResolutionError nameResolutionError)
		{
			Error(nameResolutionError.Token, nameResolutionError.Message);
		}

		void fun(string x) {
			console.Text += x + "\n";
		}

		Interpreter interpreter = new Interpreter(RuntimeError, fun);

		object? result = interpreter.Eval(txt, ScanError, ParseError, NameResolutionError);

		if (result != null && result != VoidObject.Void)
		{
			fun(result.ToString());
		}

		// Line l1 = new();
		// Line l2 = new();		
		// Line l3 = new();		
		// Line l4 = new();

		// Arc A1 = new();		
		// Arc A2 = new();		
		// Arc A3 = new();		
		// Arc A4 = new();

		// Circle C1 = new();	
		// Circle C2 = new();	
		// Circle C3 = new();	
		// Circle C4 = new();	


		// draw_area.AddDrawable(Colors.RebeccaPurple, l1, l2, l3, l4, A1, A2, A3, A4, C1, C2, C3, C4);

		// ShowIntersect(X, Y, l1, l2, l3, l4, A1, A2, A3, A4, C1, C2, C3, C4);
		// draw_area.AddDrawable(Colors.Red, l1);
		// draw_area.QueueRedraw();

		// GD.Print(txt);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {

	// }

}
