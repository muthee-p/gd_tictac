using Godot;
using System;

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
                    board[row, col] = -1; // Assume AI is player -1
                    int score = Minimax(board, 0, int.MinValue, int.MaxValue, false);
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

    private int Minimax(int[,] board, int depth, int alpha, int beta, bool isMaximizing)
    {
        int result = Evaluate(board);
        if (result != 0)
        {
            return result;
        }
        if (isBoardFull(board))
        {
            return 0;
        }

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (board[row, col] == 0)
                    {
                        board[row, col] = -1; // Assume AI is player -1
                        int score = Minimax(board, depth + 1, alpha, beta, false);
                        board[row, col] = 0;

                        bestScore = Math.Max(score, bestScore);
                        alpha = Math.Max(alpha, bestScore);

                        if (beta <= alpha)
                        {
                            break; // Beta cutoff
                        }
                    }
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (board[row, col] == 0)
                    {
                        board[row, col] = 1; // Assume Player is player 1
                        int score = Minimax(board, depth + 1, alpha, beta, true);
                        board[row, col] = 0;

                        bestScore = Math.Min(score, bestScore);
                        beta = Math.Min(beta, bestScore);

                        if (beta <= alpha)
                        {
                            break; // Alpha cutoff
                        }
                    }
                }
            }
            return bestScore;
        }
    }

    private int Evaluate(int[,] board)
    {
        int aiScore = EvaluateScore(board, -1); // AI player
        int humanScore = EvaluateScore(board, 1); // Human player

        if (aiScore == 10) return aiScore;
        if (humanScore == 10) return -humanScore;

        return 0;
    }

    private int EvaluateScore(int[,] board, int player)
    {
        for (int i = 0; i < 3; i++)
        {
            // Rows and Columns
            if ((board[i, 0] == player && board[i, 1] == player && board[i, 2] == player) ||
                (board[0, i] == player && board[1, i] == player && board[2, i] == player))
            {
                return 10;
            }
        }

        // Diagonals
        if ((board[0, 0] == player && board[1, 1] == player && board[2, 2] == player) ||
            (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player))
        {
            return 10;
        }

        return 0;
    }

    private bool isBoardFull(int[,] board)
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (board[row, col] == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
