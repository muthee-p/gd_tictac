using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Pipes;


public class minimax
{
	public Vector2 FindBestMove(int[,] board)
{
    int bestScore = int.MinValue;
    Vector2 bestMove = new Vector2(-1, -1);

    for (int row = 0; row < 3; row++)
    {
        for (int col = 0; col < 3; col++)
        {
            if (board[row, col] == 0)
            {
                board[row, col] = 1;
                int score = Minimax(board, 0, false);
                board[row, col] = 0;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = new Vector2(col, row);
                }
            }
        }
    }

    return bestMove;
}


	private int Minimax(int[,] board, int depth, bool isMaximizing){
		int result = Evaluate(board);
		if(result != 0){
			return result;
		}
		if(isBoardFull(board)){
			return 0;
		}
		if(isMaximizing){
			int bestScore=int.MinValue;
			for (int row = 0;row<3; row++){
				for (int col =0; col<3; col++){
					if (board[row,col] == 0){
						board[row, col] = 1;
						int score = Minimax(board, depth + 1, false);
						board[row, col] =0;
						bestScore = Math.Max(score, bestScore);
					}
				}
			}
			return bestScore;
		}
		else{
			int bestScore = int.MaxValue;
			for(int row = 0; row<3; row++){
				for(int col = 0; col < 3; col ++){
					if (board[row,col] == 0){
						board[row, col] = -1;
						int score = Minimax(board, depth + 1, true);
						board[row, col] =0;
						bestScore = Math.Min(score, bestScore);
					}
				}

			}
			return bestScore;
		}
	}
	private int Evaluate(int[,] board){
		return 0;
	}
	private bool isBoardFull(int[,] board){
		for(int row = 0; row<3; row++){
			for(int col = 0; col < 3; col ++){
				if (board[row,col] == 0){
					return false;
				}
			}
		}
		return true;
	}
}


