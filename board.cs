using Godot;
using System;

public partial class board : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
extends Node

var player:int
var grid_data : Array
var grid_pos: Vector2i
var board_size: int
var cell_size : int

func _ready():
board_size = $board.texture.get
cell_size = board_size /3 
new_game()

func _process(delta):
	pass

func _input(event):
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and event.pressed:
			if event.position.x < board_size:
				grid_pos = Vector2i(event.position / cell_size)
				if grid_data[grid.pos.y][grid.pos.x] ==0:
				grid_data[grid.pos.y][grid.pos.x] = player
				player *= -1

func new_game():
	player = 1
	grid_data = [[0,0,0], [0,0,0],[0,0,0]]
