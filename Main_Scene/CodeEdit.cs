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
		codeHighlighter.AddMemberKeywordColor("undefined", g);		

		g = Colors.Yellow;
		codeHighlighter.AddMemberKeywordColor("draw", g);
		codeHighlighter.AddMemberKeywordColor("print", g);
		codeHighlighter.AddMemberKeywordColor("intersect", g);

		g = Colors.MediumVioletRed;
		codeHighlighter.AddMemberKeywordColor("in", g);
		codeHighlighter.AddMemberKeywordColor("let", g);
		codeHighlighter.AddMemberKeywordColor("if", g);
		codeHighlighter.AddMemberKeywordColor("then", g);
		codeHighlighter.AddMemberKeywordColor("else", g);
		
		codeHighlighter.AddMemberKeywordColor("and", g);
		codeHighlighter.AddMemberKeywordColor("or", g);
		
		codeHighlighter.AddMemberKeywordColor("import", g);
		codeHighlighter.AddMemberKeywordColor("color", g);
		codeHighlighter.AddMemberKeywordColor("restore", g);
		
		codeHighlighter.AddColorRegion("\"", "\"", Colors.RosyBrown);

		codeHighlighter.NumberColor = Colors.SkyBlue;
		g = Colors.SkyBlue;
		codeHighlighter.AddMemberKeywordColor("true", g);
		codeHighlighter.AddMemberKeywordColor("false", g);

		codeHighlighter.SymbolColor = Colors.Red;
		codeHighlighter.FunctionColor = Colors.Yellow;

		SyntaxHighlighter = codeHighlighter;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
