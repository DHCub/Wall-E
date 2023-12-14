using Godot;
using GSharp;
using GSharp.Exceptions;
using GSharp.Objects.Collections;
using GSharp.Interpreter;
using GSharp.Objects.Figures;
using System.Collections.Generic;
using System;
using GSharp.GUIInterface;
using Microsoft.VisualBasic;
using System.IO;

public partial class Control : Godot.Control
{
	static readonly Segment S = new(new(0, 0), new(200, 200));
	public GUIInterface gsharp;
	public override void _Ready()
	{
		var draw_area = GetNode<Node2D>("Draw_Area_Marg/Viewport_Container/SubViewport/Background/Node2D");
		var console = GetNode<RichTextLabel>("Console_Margin/Console");

		void standardOutputHandler(string text)
		{
			console.Text += text + '\n';
		}
		void errorHandler(string text)
		{
			console.Text += text + '\n';
		}
		Godot.Color GetColor(GSharp.GUIInterface.Colors color) => color switch
		{
			GSharp.GUIInterface.Colors.Black => Godot.Colors.Black,
			GSharp.GUIInterface.Colors.Red => Godot.Colors.Red,
			GSharp.GUIInterface.Colors.Green => Godot.Colors.Green,
			GSharp.GUIInterface.Colors.Blue => Godot.Colors.Blue,
			GSharp.GUIInterface.Colors.Magenta => Godot.Colors.Magenta,
			GSharp.GUIInterface.Colors.Purple => Godot.Colors.Purple,
			GSharp.GUIInterface.Colors.Gray => Godot.Colors.Gray,
			GSharp.GUIInterface.Colors.Cyan => Godot.Colors.Cyan,
			GSharp.GUIInterface.Colors.Yellow => Godot.Colors.Yellow,
			GSharp.GUIInterface.Colors.White => Godot.Colors.White,

			_ => throw new NotImplementedException("UNSUPPORTED COLOR")
		};
		void drawFigure(GSharp.GUIInterface.Colors color, Figure fig)
		{
			draw_area.AddDrawable(GetColor(color), fig);
			draw_area.QueueRedraw();
		}
		void drawLabeledFigure(GSharp.GUIInterface.Colors color, Figure fig, string label)
		{
			draw_area.AddDrawable(GetColor(color), fig, label);
			draw_area.QueueRedraw();
		}
		string importHandler(string dir)
		{
			if (!Godot.FileAccess.FileExists(dir)) return null;
			var txt = Godot.FileAccess.Open(dir, Godot.FileAccess.ModeFlags.Read);
			return txt.GetAsText();
		}

		gsharp = new GUIInterface(standardOutputHandler, errorHandler, importHandler, drawFigure, drawLabeledFigure);

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
							draw_area.AddDrawable(Godot.Colors.Black, (Point)P);
					}
				}
			}
		}

		var txt = code.Text;
		
		draw_area.Clear();
		gsharp.Interpret(txt);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {

	// }

}
