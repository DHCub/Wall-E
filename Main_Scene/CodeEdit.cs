using Godot;
using System;

public partial class CodeEdit : Godot.CodeEdit
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CodeHighlighter codeHighlighter = new();
		var g = Colors.Green;
		codeHighlighter.AddMemberKeywordColor("point", g);
		codeHighlighter.AddMemberKeywordColor("line", g);
		codeHighlighter.AddMemberKeywordColor("ray", g);
		codeHighlighter.AddMemberKeywordColor("segment", g);
		codeHighlighter.AddMemberKeywordColor("circle", g);
		codeHighlighter.AddMemberKeywordColor("arc", g);
		codeHighlighter.AddMemberKeywordColor("sequence", g);		

		g = Colors.Yellow;
		codeHighlighter.AddMemberKeywordColor("draw", g);
		codeHighlighter.AddMemberKeywordColor("print", g);
		codeHighlighter.AddMemberKeywordColor("intersect", g);
		
		codeHighlighter.AddColorRegion("\"", "\"", Colors.RosyBrown);

		codeHighlighter.NumberColor = Colors.SkyBlue;
		codeHighlighter.SymbolColor = Colors.Red;
		codeHighlighter.FunctionColor = Colors.Yellow;

		SyntaxHighlighter = codeHighlighter;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
