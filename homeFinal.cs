using Godot;
using System;

public partial class homeFinal : Button
{
	private void on_pressed(){
		
		GetTree().ChangeSceneToFile("res://menu.tscn");
	}
	
}
