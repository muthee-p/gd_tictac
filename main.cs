using Godot;
using System;

public partial class main : Node2D
{
	
	private int player;
	private Array gridData;
	private Vector2i gridPos;
	private int boardSize;
	private int cellSize;
	
	public override void _Ready()
	{
		boardSize = GetNode<TextureRect>("board").Texture.GetSize().Width;
		cellSize = boardSize / 3;
		NewGame();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _Input(InputEvent event)
	{
		if (event is InputEventMouseButton && event.Pressed && event.ButtonIndex == MouseButton.Left)
		{
			Vector2 mousePos = event.Position;

			// Calculate the grid position of the mouse click.
			gridPos = new Vector2i((int)(mousePos.x / cellSize), (int)(mousePos.y / cellSize));

			// Check if the grid cell is empty and place the player's mark.
			if (gridData[gridPos.y, gridPos.x] == 0)
			{
				gridData[gridPos.y, gridPos.x] = player;
				player *= -1;
			}
		}
	}

	private void NewGame()
	{
		player = 1;
		gridData = new int[3, 3];
	}
}
