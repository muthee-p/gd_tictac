using Godot;
using System;

public partial class menu : Control
{
	private void _on_single_player_pressed(){
 		GetTree().ChangeSceneToFile("res://compGame.tscn");
	}
	private void _on_two_player_pressed(){
 		GetTree().ChangeSceneToFile("res://game.tscn");
	}
	private void _on_exit_pressed(){
 		GetTree().Quit();
	}
}

