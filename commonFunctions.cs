using Godot;
using System;
using System.Collections.Generic;
using System.IO.Pipes;


public partial class commonFunctions : Node
{
	private int player;
	//private Panel turnDots;
	//private Vector2 turnDotsPosition;
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
	private int player1Score;
	private int player2Score;

	
	private void NewGame(int[,] gridData){
		player = 1;
		gridData = new int[3,3];
		gameOver.Visible = false;
		rowSum=0;
		colSum=0;
		diag1Sum=0;
		diag2Sum=0;
		greenDot.Visible = true;
		player1Score =0;
		player2Score = 0;

		GetTree().CallGroup("circles", "QueueFree");
		GetTree().CallGroup("crosses", "QueueFree");
		//CreateMarker(player, turnDotsPosition + new Vector2(cellSize/2, cellSize/2), true);
    
	}
	

	public int CheckWin(int[,] gridData){
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




