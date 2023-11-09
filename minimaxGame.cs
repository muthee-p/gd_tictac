using Godot;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading.Tasks;


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
	private Sprite2D blackDot;
	private Sprite2D greenDot2;
	private Sprite2D blackDot2;
	private Sprite2D rowWin;
	private Sprite2D colWin;
	private Sprite2D diag1Win;
	private Sprite2D diag2Win;
	private int rowSum;
	private int colSum;
	private int diag1Sum;
	private int diag2Sum;
	private int winner;
	private int moves;
	private Label resultLabel;
	private const int MOUSE_BUTTON_LEFT = 1;
	private minimax minimax;
	private int player1Score;
	private int player2Score;
	private AudioStreamPlayer2D gameOverSound;
	private Label turnLabel;
	private Label player1ScoreLabel;
	private Label player2ScoreLabel;
	private bool isPlayerTurn;
	private Sprite2D soundOnIcon;
	private Sprite2D muteIcon;
    private Button soundOn;
    private bool isMasterAudioMuted;

	public override void _Ready()
	{
		player1Score = 0;
		player2Score = 0;
		//player = 1;
		boardSize = 640;
		cellSize = 145;
		winner = 0;
		gameOver = GetNode("gameOver") as CanvasLayer;
		resultLabel = gameOver.GetNode("resultLabel") as Label;
		greenDot = GetNode("greenDot") as Sprite2D;
		blackDot = GetNode("blackDot") as Sprite2D;
		greenDot2 = GetNode("greenDot2") as Sprite2D;
		blackDot2 = GetNode("blackDot2") as Sprite2D;
		colWin = GetNode("colWin") as Sprite2D;
		rowWin = GetNode("rowWin") as Sprite2D;
		diag1Win = GetNode("diag1Win") as Sprite2D;
		diag2Win = GetNode("diag2Win") as Sprite2D;
		gameOverSound = GetNode("gameOverSound") as AudioStreamPlayer2D;
		turnLabel = GetNode("turnLabel") as Label;
		player1ScoreLabel = GetNode("player1ScoreLabel") as Label;
		player2ScoreLabel = GetNode("player2ScoreLabel") as Label;
		greenDot.Position = new Vector2(90, 670);
		greenDot2.Position = new Vector2(580, 670);
		blackDot.Position = new Vector2(90, 670);
		blackDot2.Position = new Vector2(580, 670);
		muteIcon = GetNode("muteIcon") as Sprite2D;
    	soundOnIcon = GetNode("soundOnIcon") as Sprite2D;
		muteIcon.Visible= false;
		soundOnIcon.Visible= true;

		minimax = new minimax();
		NewGame();
	}

	public override void _Process(double delta)
	{
		//CheckWin();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			if ((int)mouseButtonEvent.ButtonIndex == 1 && mouseButtonEvent.Pressed)
			{
				if (isPlayerTurn && !GetTree().Paused)
				{
					Vector2 mousePosition = mouseButtonEvent.Position;
					if (mousePosition.X < boardSize)
					{
						gridPos = new Pos((int)((mousePosition.X - 150) / cellSize), (int)((mousePosition.Y - 485) / cellSize));
						if (gridData[gridPos.y, gridPos.x] == 0)
						{

							moves += 1;
							gridData[gridPos.y, gridPos.x] = player;

							Vector2 makerPosition = new Vector2(205 + gridPos.x * cellSize, 530 + gridPos.y * cellSize);
							CreateMarker(player, makerPosition);
							isPlayerTurn = false;
							Turn();
							player = -1;
							ComputerMove();

							CheckTie();

							if (CheckWin() != 0)
							{
								GetTree().Paused = true;
								gameOverSound.Play();
								gameOver.Visible = true;
								if (winner == 1)
								{
									resultLabel.Text = "Cross Wins";
								}
								SceneTreeTimer resetGameTimer = GetTree().CreateTimer(2);
								Callable resetGame = new Callable(this, "ResetGame");
								resetGameTimer.Connect("timeout", resetGame);
							}


						}
					}

				}
			}
		}
	}


	private async void ComputerMove()
	{
		if (!isPlayerTurn)
		{
			await ToSignal(GetTree().CreateTimer(1), "timeout");
			Vector2 bestMove = minimax.FindBestMove(gridData);
			//greenDot.Visible = true;
			int row = (int)bestMove.Y;
			int col = (int)bestMove.X;

			moves += 1;
			gridData[row, col] = player;

			Vector2 makerPosition = new Vector2(185 + col * cellSize, 540 + row * cellSize);

			CreateMarker(player, makerPosition);
			isPlayerTurn = true;
			Turn();
			player = 1;

			CheckTie();

			if (CheckWin() != 0)
			{
				GetTree().Paused = true;

				gameOver.Visible = true;
				if (winner == -1)
				{
					resultLabel.Text = "Circle Wins";
				}
				SceneTreeTimer resetGameTimer = GetTree().CreateTimer(2);
				Callable resetGame = new Callable(this, "ResetGame");
				resetGameTimer.Connect("timeout", resetGame);
			}
		}
	}

	private void Turn()
	{
		if (player == -1)
		{

			turnLabel.Text = "Player's turn";
			greenDot.Visible = true;
			blackDot.Visible = false;
			blackDot2.Visible = true;
			greenDot2.Visible = false;


		}
		else if (player == 1)
		{
			turnLabel.Text = "Computer's turn";
			greenDot2.Visible = true;
			blackDot.Visible = true;
			greenDot.Visible = false;
			blackDot2.Visible = false;
		}
	}


	private void ResetGame()
	{
		NewGame();
		moves = 0;
		GetTree().Paused = false;
		gameOver.Visible = false;
		resultLabel.Text = "";
	}
	private void NewGame()
	{
		player = (player == 1) ? -1 : 1;
		gridData = new int[3, 3];
		gameOver.Visible = false;
		rowSum = 0;
		colSum = 0;
		diag1Sum = 0;
		diag2Sum = 0;
		colWin.Visible = false;
		rowWin.Visible = false;
		diag1Win.Visible = false;
		diag2Win.Visible = false;

		Node2D markerParent = GetNode("dotsCrosses") as Node2D;
		Godot.Collections.Array<Node> markers = markerParent.GetChildren();
		for (int i = 0; i < markers.Count; i++)
		{
			markers[i].QueueFree();
		}

		Turn();

		if (player == -1)
		{
			isPlayerTurn = false;
			ComputerMove();

		}
		if (player == 1)
		{
			isPlayerTurn = true;
		}


	}

	private void CreateMarker(int player, Vector2 position) //bool turn=false)
	{
		Node2D markerParent = GetNode("dotsCrosses") as Node2D;
		if (player == 1)
		{
			AudioStreamPlayer2D clickSound = GetNode("clickSound") as AudioStreamPlayer2D;
			clickSound.Play();

			crossScene = ResourceLoader.Load("res://cross.tscn") as PackedScene;
			Sprite2D cross = crossScene.Instantiate() as Sprite2D;
			cross.Position = position;
			markerParent.AddChild(cross);
		}
		if (player == -1)
		{
			AudioStreamPlayer2D clickSound = GetNode("clickSound") as AudioStreamPlayer2D;
			clickSound.Play();

			circleScene = ResourceLoader.Load("res://circle.tscn") as PackedScene;
			Sprite2D circle = circleScene.Instantiate() as Sprite2D;
			circle.Position = position;

			markerParent.AddChild(circle);
		}
	}
	private int CheckWin()
	{
		for (int i = 0; i < gridData.Length; i++)
		{
			rowSum = gridData[i, 0] + gridData[i, 1] + gridData[i, 2];
			colSum = gridData[0, i] + gridData[1, i] + gridData[2, i];
			diag1Sum = gridData[0, 0] + gridData[1, 1] + gridData[2, 2];
			diag2Sum = gridData[0, 2] + gridData[1, 1] + gridData[2, 0];

			if (diag1Sum == 3 || diag1Sum == -3)
			{
				diag1Win.Visible = true;
			}
			else if (diag2Sum == 3 || diag2Sum == -3)
			{
				diag2Win.Visible = true;

			}
			else if (colSum == 3 || colSum == -3)
			{
				colWin.Position = new Vector2(cellSize * i + 200, 670);
				colWin.Visible = true;
			}

			else if (rowSum == 3 || rowSum == -3)
			{
				rowWin.Visible = true;
				rowWin.Position = new Vector2(350, cellSize * i + 520);
			}




			if (rowSum == 3 || colSum == 3 || diag1Sum == 3 || diag2Sum == 3)
			{

				player1Score += 1;
				player1ScoreLabel.Text = "" + player1Score;
				winner = 1;
				return 1;
			}
			else if (rowSum == -3 || colSum == -3 || diag1Sum == -3 || diag2Sum == -3)
			{

				player2Score += 1;
				player2ScoreLabel.Text = "" + player2Score;
				winner = -1;
				return -1;

			}
		}
		return winner;
	}

	private void CheckTie()
	{
		if (moves == 9)
		{
			player1Score += 1;
			player1ScoreLabel.Text = "" + player1Score;
			player2Score += 1;
			player2ScoreLabel.Text = "" + player2Score;
			gameOverSound.Play();
			gameOver.Visible = true;
			//game_over = true;
			resultLabel.Text = "Its a tie";

			SceneTreeTimer resetGameTimer = GetTree().CreateTimer(2);
			Callable resetGame = new Callable(this, "ResetGame");
			resetGameTimer.Connect("timeout", resetGame);
		}
	}
	private void _on_home_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://menu.tscn");
	}
	private void _on_easy_play_pressed()
	{
		GetTree().ChangeSceneToFile("res://compGame.tscn");
	}
	private void _on_sound_on_pressed()
	{ 
		//musicBus = AudioServer.GetBusIndex("music");
		isMasterAudioMuted = !isMasterAudioMuted;

    AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), isMasterAudioMuted);
		if(isMasterAudioMuted){
			muteIcon.Visible= true;
			soundOnIcon.Visible= false;
		}
		else if(!isMasterAudioMuted){
			muteIcon.Visible= false;
			soundOnIcon.Visible= true;
		}
	}
}

