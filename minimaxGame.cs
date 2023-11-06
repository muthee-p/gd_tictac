using Godot;
using System;
using System.Collections.Generic;
using System.IO.Pipes;


public partial class minimaxGame : Node2D
{
	private int player;

	private int[,] gridData;
    private CanvasLayer gameOver;
    private Pos gridPos;
	private int boardSize;
	private int cellSize;
	private PackedScene circleScene; 
	private PackedScene crossScene;
	private Sprite2D greenDot;
	private int rowSum;
	private int colSum;
	private int diag1Sum;
	private int diag2Sum;
	private int winner;
	private int moves;
    private Label resultLabel;
    private const int MOUSE_BUTTON_LEFT= 1;
	private minimax minimax;

    public override void _Ready()
	{
		player = 1;
		boardSize = 640;
		cellSize = 160;
		winner = 0;
		gameOver = GetNode("gameOver") as CanvasLayer;
		resultLabel = gameOver.GetNode("resultLabel") as Label;
		greenDot = GetNode("greenDot") as Sprite2D;
		greenDot.Position = new Vector2(90,665);
		greenDot.Visible = true;
		minimax = new minimax();
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
					gridPos = new Pos((int)((mousePosition.X -150) / 145), (int)((mousePosition.Y-485) / 145));
					if (gridData[gridPos.y, gridPos.x] == 0)
					{
						//greenDot.Visible = false;
						moves += 1;
						gridData[gridPos.y, gridPos.x] = player;
						
						//player *= -1;
						
						
						Vector2 makerPosition = new Vector2(185+gridPos.x * cellSize, 540+gridPos.y * cellSize);
                    	CreateMarker(player, makerPosition);
						player =-1;
						
						if(player == -1){
							ComputerMove();
						}

						//player=1;
						GD.Print(player);

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
						
							
					//CreateMarker(player, turnDots + new Vector2(cellSize/2,cellSize/2));
					}
				}
			}
		}
	}


	private void ComputerMove()
	{
		Vector2 bestMove =minimax.FindBestMove(gridData);
		//greenDot.Visible = true;
		int row = (int)bestMove.Y;
		int col = (int)bestMove.X;

		gridData[row, col] = player;
			
			
		Vector2 makerPosition = new Vector2(185+ col * cellSize, 540 + row * cellSize);
		GD.Print(makerPosition);
		CreateMarker(player, makerPosition);

		player =1;
		//player *= -1;
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

			//player = 1;
	}
	
	

	private void NewGame(){
		player = 1;
		gridData = new int[3,3];
		gameOver.Visible = false;
		rowSum=0;
		colSum=0;
		diag1Sum=0;
		diag2Sum=0;
		greenDot.Visible = true;
		GetTree().CallGroup("circles", "QueueFree");
		GetTree().CallGroup("crosses", "QueueFree");
		//CreateMarker(player, turnDotsPosition + new Vector2(cellSize/2, cellSize/2), true);
    
	}
	private void CreateMarker(int player, Vector2 position) //bool turn=false)
	{
		if (player == 1)
		{
		circleScene = ResourceLoader.Load("res://circle.tscn") as PackedScene;
        Sprite2D circle = circleScene.Instantiate() as Sprite2D;
        circle.Position= position;
        AddChild(circle);
		}
		if(player == -1)
		{
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
				winner = 1;
				return 1;
			}else if(rowSum == -3 || colSum == -3 || diag1Sum == -3 || diag2Sum == -3){
				winner = -1;
				return -1;
			}
		}
		return winner;
	}	
}

