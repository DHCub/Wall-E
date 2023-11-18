using Godot;
using Geometry;



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

		// var txt = code.Text;
		// var data = txt.Split('\n');
		// var v1 = double.Parse(data[0]);
		// var v2 = -double.Parse(data[1]);
		// var rad = double.Parse(data[2]);

		// draw_area.Clear();
		draw_area.AddDrawable(
			new Arc(
				new(new(0, 0), new(1, 1)),
				new(new(0, 0), new(0, 1)),
				100
			),
			new(1000)
		);
		draw_area.AddDrawable(
			new Line(new(0, 0), new(1, 0)),
			new(1000)
		);
		draw_area.AddDrawable(
			new Ray(
				new(0, 0),
				new(1, 1)
			),
			new(1000)
		);
		draw_area.AddDrawable(
			new Segment(
				new(-100, 100),
				new(-100, -200)
			),
			new(1000)
		);
		draw_area.AddDrawable(
			new Point(500, -100),
			new(1000)
		);
		draw_area.AddDrawable(
			new Circle(
				new(-100, 100),
				200
			),
			new(1, 0, 0)
		);

		GD.Print(new Point(1, 0).AngleTo(new Point(1, 0)));

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

		GeoExpr.UpdateWindow(
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

