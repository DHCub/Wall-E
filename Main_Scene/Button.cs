using Godot;
using System;

public class Button : Godot.Button
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // var button = new Button();
        // button.Pressed = false;
        this.Text = "Draw";
        // AddChild(button);
        Connect("pressed", this, "ButtonPressed");
    }

    public void ButtonPressed()
    {
        
    }


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
