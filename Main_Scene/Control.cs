using Godot;
using System;

public class Control : Godot.Control
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}


	public void _on_Button_pressed() 
	{
		var code = GetNode<TextEdit>("MarginContainer/TextEdit");
		var draw_area = GetNode<Node2D>("ViewportContainer/Viewport/Node2D");

		var txt = code.Text;
		var data = txt.Split('\n');
		var v1 = float.Parse(data[0]);
		var v2 = float.Parse(data[1]);
		var rad = float.Parse(data[2]);

		draw_area.shapes.Add(new Circle(new Vector2(v1, v2), rad, new Color(100)));
		draw_area.draw();
		GD.Print(txt);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
