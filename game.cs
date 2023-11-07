using Godot;
using System;
using System.IO.Pipes;


public partial class game : Node2D
{
	private int player;
	private Panel turnDots;
	private Vector2 turnDotsPosition;
	private int[,] gridData;
    private CanvasLayer gameOver;
    private Pos gridPos;
	private int boardSize;
	private int cellSize;
	private PackedScene circleScene; 
	private PackedScene crossScene;
	private int rowSum;
	private int colSum;
	private int diag1Sum;
	private int diag2Sum;
	private int winner;
	private int moves;
    private Label resultLabel;
    private int player1Score;
    private int player2Score;
    private const int MOUSE_BUTTON_LEFT= 1;

    public override void _Ready()
	{
		player1Score =0;
		player2Score = 0;
		boardSize = 640;
		cellSize = boardSize / 3;
		winner = 0;
		gameOver = GetNode("gameOver") as CanvasLayer;
		resultLabel = gameOver.GetNode("resultLabel") as Label;
		//turnDotsPosition = turnDots.GlobalPosition;
		NewGame();
	}

	public override void _Process(double delta)
	{
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			if ((int)mouseButtonEvent.ButtonIndex == 1 && mouseButtonEvent.Pressed)
			{
				Vector2 mousePosition = mouseButtonEvent.Position;
				if (mousePosition.X < boardSize)
				{
					gridPos = new Pos((int)(mousePosition.X / cellSize), (int)(mousePosition.Y / cellSize));
					if (gridData[gridPos.y, gridPos.x] == 0)
					{
						moves += 1;
						gridData[gridPos.y, gridPos.x] = player;
						player *= -1;

						Vector2 makerPosition = new Vector2(gridPos.x * cellSize, gridPos.y * cellSize);
                    	CreateMarker(player, makerPosition);
						if(CheckWin() != 0){
							GetTree().Paused= true;
							gameOver.Visible = true;
							if(winner == 1){
								resultLabel.Text=" Player 1";
							}else if(winner == -1){
								resultLabel.Text = "Player 2";
							}
						}else if(moves ==9 ){
							gameOver.Visible = true;
							resultLabel.Text = "Its a tie";
						}
					//CreateMarker(player, turnDotsPosition + new Vector2(cellSize/2,cellSize/2));
					}
				}
			}
		}
	}

	private void NewGame(){
		player = 1;
		gridData = new int[3,3];
		gameOver.Visible = false;
		rowSum=0;
		colSum=0;
		diag1Sum=0;
		diag2Sum=0;
		GetTree().CallGroup("circles", "QueueFree");
		GetTree().CallGroup("crosses", "QueueFree");
		//CreateMarker(player, turnDotsPosition + new Vector2(cellSize/2, cellSize/2), true);
    
	}
	private void CreateMarker(int player, Vector2 position) //bool turn=false)
	{
		if (player == -1)
		{
		circleScene = ResourceLoader.Load("res://circle.tscn") as PackedScene;
        Sprite2D circle = circleScene.Instantiate() as Sprite2D;
        circle.Position= position;
		AudioStreamPlayer2D clickSound = GetNode("clickSound") as AudioStreamPlayer2D;
		clickSound.Play();
        AddChild(circle);
		}
		if(player == 1)
		{
			AudioStreamPlayer2D clickSound = GetNode("clickSound") as AudioStreamPlayer2D;
			clickSound.Play();
			crossScene = ResourceLoader.Load("res://cross.tscn") as PackedScene;
        	Sprite2D cross = crossScene.Instantiate() as Sprite2D;
        	cross.Position = position;
        	AddChild(cross);
		}
	}

	private int CheckWin(){
		for(int i=0; i<gridData.Length; i++){
			 rowSum = gridData[i,0]+gridData[i,1] + gridData[i,2];
			 colSum = gridData[0,i]+gridData[1, i] + gridData[2, i];
			 diag1Sum = gridData[0,0]+gridData[1, 1] + gridData[2,2];
			 diag2Sum = gridData[0,2]+gridData[1,1] + gridData[2,0];
		
			if(rowSum == 3 || colSum == 3 || diag1Sum == 3 || diag2Sum == 3){
				player1Score +=1;
				AudioStreamPlayer2D gameOverSound = GetNode("gameOverSound") as AudioStreamPlayer2D;
				gameOverSound.Play();
				winner = 1;
				return 1;
			}else if(rowSum == -3 || colSum == -3 || diag1Sum == -3 || diag2Sum == -3){
				player2Score +=1;
				AudioStreamPlayer2D gameOverSound = GetNode("gameOverSound") as AudioStreamPlayer2D;
				gameOverSound.Play();
				winner = -1;
				return -1;
			}
		}
		return winner;
	}	
}

public class Pos
{
	public int x;
	public int y;

	public Pos(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	// public void Move(int x, int y)
	// {
	// 	this.x += x;
	// 	this.y += y;
	// }
}


