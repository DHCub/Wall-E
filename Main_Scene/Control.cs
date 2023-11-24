using Godot;
using Geometry;
using System;
using GSharp;
using System.Collections.Generic;



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

		void ShowIntersect(params GeoExpr[] arr)
		{
			for (int i = 0; i < arr.Length - 1; i++)
			{
				for (int j = i + 1; j < arr.Length; j++)
				{
					if (i == j) continue;
					var intersect = Functions.Intersect(arr[i], arr[j]);
				
					if (intersect is Finite_Static_Seqence<Point> Seq)
					{
						foreach(var P in Seq)
							draw_area.AddDrawable(Colors.Black, P);
					}
				}
			}
		}

		void WriteErrors(CollectorLogger logger)
		{
			List<char> ErrorMsg = new();

			foreach(var error in logger.Errors)
				ErrorMsg.AddRange(error + '\n');

			console.Text = new string(ErrorMsg.ToArray());
		}

		var txt = code.Text;

		var logger = new CollectorLogger();
		var scanner = new Scanner(logger, txt);
		
		var parser = new Parser(logger, scanner.ScanTokens());
		
		if(logger.hadError)
		{
			WriteErrors(logger);
			return;
		}
		
		var analyzer = new Semantic_Analyzer(logger, parser.Parse());
		if(logger.hadError)
		{
			WriteErrors(logger);
			return;
		}


		analyzer.Analyze();
		if (logger.hadError)
		{
			WriteErrors(logger);
			return;
		}

		var X = new Line(new(0, 0), new(1, 0));
		var Y = new Line(new(0, 0), new(0, 1));



		Line l1 = new();		
		Line l2 = new();		
		Line l3 = new();		
		Line l4 = new();

		Arc A1 = new();		
		Arc A2 = new();		
		Arc A3 = new();		
		Arc A4 = new();

		Circle C1 = new();	
		Circle C2 = new();	
		Circle C3 = new();	
		Circle C4 = new();	

		draw_area.AddDrawable(Colors.Red, X, Y);
		draw_area.AddDrawable(Colors.RebeccaPurple, l1, l2, l3, l4, A1, A2, A3, A4, C1, C2, C3, C4);

		ShowIntersect(X, Y, l1, l2, l3, l4, A1, A2, A3, A4, C1, C2, C3, C4);

		draw_area.QueueRedraw();

		GD.Print("drawn");
		// GD.Print(txt);
	}

 	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
		
	// }

}

