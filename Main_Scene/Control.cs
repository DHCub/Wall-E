using Godot;
using Geometry;
using System;
using GSharp;



public partial class Control : Godot.Control
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Center_Transform();
		Update_GeoExpr_Window();
	}


	public void _on_Button_pressed() 
	{

		var code = GetNode<TextEdit>("Code_Edit_Marg/Code_Edit");
		var draw_area = GetNode<Node2D>("Draw_Area_Marg/Viewport_Container/SubViewport/Background/Node2D");

		void ShowIntersect(params GeoExpr[] arr)
		{
			for (int i = 0; i < arr.Length - 1; i++)
			{
				for (int j = i + 1; j < arr.Length; j++)
				{
					var intersect = Functions.Intersect(arr[i], arr[j]);

					if (intersect is Finite_Static_Seqence<Point> Seq)
					{
						draw_area.AddDrawable(Colors.Red, Seq.GetRemainder(0));
					}
				}
			}
		}

		// var txt = code.Text;
		// var data = txt.Split('\n');
		// var v1 = double.Parse(data[0]);
		// var v2 = -double.Parse(data[1]);
		// var rad = double.Parse(data[2]);

		draw_area.Clear();
		
		Point p1 = new(), p2 = new();

		Line Base_Line = new(p1, p2);

		var Height = Ray.Point_DirectorVec(p2, Base_Line.Direction_Vector.Orthogonal());

		var m = p1.Distance_To(p2);

		Circle C1 = new(p2, m);

		Point p3 = Functions.Intersect(C1, Height)[0];
		Point p4 = p3 + p1 - p2;

		draw_area.AddDrawable(Colors.RebeccaPurple, p1, p2, p3, p4);

		Segment S1 = new(p1, p2);
		Segment S2 = new(p2, p3);
		Segment S3 = new(p3, p4);
		Segment S4 = new(p4, p1);

		draw_area.AddDrawable(Colors.Green, S1, S2, S3, S4);

		draw_area.QueueRedraw();
		// GD.Print(txt);
	}

 	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
		
	// }



	private void Update_GeoExpr_Window()
	{
		var draw_area_container = GetNode<MarginContainer>("Draw_Area_Marg");		

		IDrawable.UpdateWindow(
			-draw_area_container.Size.X/2, draw_area_container.Size.X/2,
			-draw_area_container.Size.Y/2, draw_area_container.Size.Y/2
		);
	}

	private void Center_Transform()
	{
		var draw_area_container = GetNode<MarginContainer>("Draw_Area_Marg");
		var draw_area = GetNode<Node2D>("Draw_Area_Marg/Viewport_Container/SubViewport/Background/Node2D");

		var x = draw_area_container.Size.X/2;
		var y = draw_area_container.Size.Y/2;


		draw_area.Transform = new Transform2D(new Vector2(1, 0), new Vector2(0, -1), new Vector2(x, y));
	}

	void _on_draw_area_marg_item_rect_changed()
	{
		Center_Transform();
		Update_GeoExpr_Window();
	}

}

