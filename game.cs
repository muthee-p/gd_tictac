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
	private const int MOUSE_BUTTON_LEFT = 1;
	private Label player1ScoreLabel;
	private Label player2ScoreLabel;
	private Label turnLabel;
	private Sprite2D greenDot;
	private Sprite2D blackDot;
	private Sprite2D greenDot2;
	private Sprite2D blackDot2;
	private Sprite2D rowWin;
	private Sprite2D colWin;
	private Sprite2D diag1Win;
	private Sprite2D diag2Win;
	private Sprite2D soundOnIcon;
	private Sprite2D muteIcon;
	private AudioStreamPlayer gameOverSound;
	private AudioStreamPlayer clickSound;
    private Button soundOn;
    private bool isMasterAudioMuted;

    public override void _Ready()
	{
		player1Score = 0;
		player2Score = 0;
		boardSize = 640;
		cellSize = 145;
		winner = 0;
		gameOver = GetNode("gameOver") as CanvasLayer;
		colWin = GetNode("colWin") as Sprite2D;
		rowWin = GetNode("rowWin") as Sprite2D;
		diag1Win = GetNode("diag1Win") as Sprite2D;
		diag2Win = GetNode("diag2Win") as Sprite2D;
		//soundOn = GetNode("soundOn") as TextureButton;
		muteIcon = GetNode("muteIcon") as Sprite2D;
    	soundOnIcon = GetNode("soundOnIcon") as Sprite2D;
		resultLabel = gameOver.GetNode("resultLabel") as Label;
		player1ScoreLabel = GetNode("player1ScoreLabel") as Label;
		player2ScoreLabel = GetNode("player2ScoreLabel") as Label;
		turnLabel = GetNode("turnLabel") as Label;
		greenDot = GetNode("greenDot") as Sprite2D;
		blackDot = GetNode("blackDot") as Sprite2D;
		greenDot2 = GetNode("greenDot2") as Sprite2D;
		blackDot2 = GetNode("blackDot2") as Sprite2D;
		gameOverSound = GetNode("gameOverSound") as AudioStreamPlayer;
		clickSound = GetNode("clickSound") as AudioStreamPlayer;
		greenDot.Position = new Vector2(90, 670);
		greenDot2.Position = new Vector2(580, 670);
		blackDot.Position = new Vector2(90, 670);
		blackDot2.Position = new Vector2(580, 670);		
		muteIcon.Visible= false;
		soundOnIcon.Visible= true;
		

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
					gridPos = new Pos((int)((mousePosition.X - 150) / cellSize), (int)((mousePosition.Y - 485) / cellSize));
					if (gridData[gridPos.y, gridPos.x] == 0)
					{
						moves += 1;
						gridData[gridPos.y, gridPos.x] = player;
						Turn();
						player *= -1;

						Vector2 makerPosition = new Vector2(205 + gridPos.x * cellSize, 530 + gridPos.y * cellSize);
						CreateMarker(player, makerPosition);

						if (moves == 9)
						{
							player1Score += 1;
							player1ScoreLabel.Text = "" + player1Score;
							player2Score += 1;
							player2ScoreLabel.Text = "" + player2Score;
							GetTree().Paused = true;


							gameOverSound.Play();

							gameOver.Visible = true;
							resultLabel.Text = "Its a tie";
							SceneTreeTimer resetGameTimer = GetTree().CreateTimer(2);
							Callable resetGame = new Callable(this, "ResetGame");
							resetGameTimer.Connect("timeout", resetGame);
						}
						else if (CheckWin() != 0)
						{
							GetTree().Paused = true;
							gameOver.Visible = true;

							if (winner == 1)
							{
								resultLabel.Text = " Cross Wins";
							}
							else if (winner == -1)
							{
								resultLabel.Text = "Circle Wins";
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

	private void Turn()
	{
		if (player == -1)
		{

			turnLabel.Text = "Circle's turn";
			greenDot2.Visible = true;
			blackDot.Visible = true;
			greenDot.Visible = false;
			blackDot2.Visible = false;

		}
		else if (player == 1)
		{
			turnLabel.Text = "Cross's turn";
			

			greenDot.Visible = true;
			blackDot.Visible = false;
			blackDot2.Visible = true;
			greenDot2.Visible = false;

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

		if (player == 1)
		{

			turnLabel.Text = "Circle's turn";
			greenDot2.Visible = true;
			blackDot.Visible = true;
			greenDot.Visible = false;
			blackDot2.Visible = false;


		}
		else if (player == -1)
		{
			turnLabel.Text = "Cross's turn";
			

			greenDot.Visible = true;
			blackDot.Visible = false;
			blackDot2.Visible = true;
			greenDot2.Visible = false;
		}

	}
	private void CreateMarker(int player, Vector2 position) //bool turn=false)
	{
		Node2D markerParent = GetNode("dotsCrosses") as Node2D;
		if (player == 1)
		{

			clickSound.Play();

			crossScene = ResourceLoader.Load("res://cross.tscn") as PackedScene;
			Sprite2D cross = crossScene.Instantiate() as Sprite2D;
			cross.Position = position;
			markerParent.AddChild(cross);
		}
		if (player == -1)
		{

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


				gameOverSound.Play();

				player2Score += 1;
				player2ScoreLabel.Text = "" + player2Score;
				winner = 1;
				return 1;
			}
			else if (rowSum == -3 || colSum == -3 || diag1Sum == -3 || diag2Sum == -3)
			{

				gameOverSound.Play();

				player1Score += 1;
				player1ScoreLabel.Text = "" + player1Score;
				winner = -1;
				return -1;
			}
		}
		return winner;
	}
	private void _on_home_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://menu.tscn");
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


