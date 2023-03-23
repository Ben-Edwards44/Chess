using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class minimax : MonoBehaviour
{
    public float maxMoveTime;

    private bool searchCaps;

    private int maxDepth;

    private DateTime time;

    private Evaluation evaluator;
    private Board mainBoard;
    private checkmate mate;
    private gameOverHandeler gameOverHandeler;

    public Text depthText;


    private void Start()
    {
        evaluator = FindObjectOfType<Evaluation>();
        mainBoard = FindObjectOfType<Board>();
        mate = FindObjectOfType<checkmate>();
        gameOverHandeler = FindObjectOfType<gameOverHandeler>();
    }


    public Board.move calculateBestMove(bool whiteToMove)
    {
        int pieceColour;

        if (whiteToMove)
        {
            pieceColour = Piece.white;
        }
        else
        {
            pieceColour = Piece.black;
        }

        return iterativeDeepening(whiteToMove, pieceColour);
    }

    private Board.move iterativeDeepening(bool whiteToMove, int pieceColour)
    {
        List<Board.move> initialMoves = mainBoard.calculateAllMoves(pieceColour, false);
        int[] moveEvals = new int[initialMoves.Count];

        int bestEval = -100000;
        Board.move bestMove = new Board.move();

        time = DateTime.Now;

        for (int depth = 1; depth <= 50; depth += 1)
        {
            initialMoves = orderFromPrev(initialMoves, moveEvals);
            maxDepth = depth;

            if ((DateTime.Now - time).TotalSeconds >= maxMoveTime)
            {
                break;
            }

            for (int i = 0; i < initialMoves.Count; i++)
            {
                Board.move move = initialMoves[i];

                int prev = boardHandeler.storedBoard[move.endIndex];

                boardHandeler.makeMoveMove(move);

                int currentEval = -search(0, !whiteToMove, -10000, 10000);

                moveEvals[i] = currentEval;

                if (currentEval > bestEval)
                {
                    bestEval = currentEval;
                    bestMove = move;
                }

                boardHandeler.unmakeMove(move, prev);
                mainBoard.revertCastle();
            }
        }

        if (maxDepth < 50)
        {
            depthText.text = "Depth Searched: " + maxDepth.ToString();
        }
        else
        {
            depthText.text = "Checkmate!";
            depthText.color = Color.magenta;

            gameOverHandeler.showEndScreen();
        }

        float t = 0;

        if (maxDepth <= 2)
        {
            t = 0;
        }
        else if (maxDepth <= 3)
        {
            t = 0.5f;
        }
        else if (maxDepth <= 4)
        {
            t = 0.75f;
        }
        else
        {
            t = 1;
        }

        depthText.color = Color.Lerp(Color.yellow, Color.green, t);

        return bestMove;
    }


    private List<Board.move> orderFromPrev(List<Board.move> moves, int[] evals)
    {
        for (int i = 0; i < moves.Count - 1; i++)
        {
            for (int j = i + 1; j > 0; j--)
            {
                int swapIndex = j - 1;

                if (evals[swapIndex] < evals[j])
                {
                    (moves[j], moves[swapIndex]) = (moves[swapIndex], moves[j]);
                    (evals[j], evals[swapIndex]) = (evals[swapIndex], evals[j]);
                }
            }
        }

        return moves;
    }


    int search(int depth, bool whiteToMove, int alpha, int beta)
    {
        if (depth > maxDepth)
        {
            if (searchCaps)
            {
                return searchCaptures(alpha, beta, whiteToMove);
            }
            else
            {
                return evaluator.evaluate(whiteToMove);
            }
        }

        int pieceColour;

        if (whiteToMove)
        {
            pieceColour = Piece.white;
        }
        else
        {
            pieceColour = Piece.black;
        }

        List<Board.move> moves = mainBoard.calculateAllMoves(pieceColour, false);
        
        if (moves.Count == 0)
        {
            if (mate.kingInCheck(pieceColour))
            {
                return -10000;
            }
            else
            {
                return 0;
            }
        }

        foreach (Board.move move in moves)
        {
            int prev = boardHandeler.storedBoard[move.endIndex];

            boardHandeler.makeMoveMove(move);

            int currentEval = -search(depth + 1, !whiteToMove, -beta, -alpha);

            boardHandeler.unmakeMove(move, prev);
            mainBoard.revertCastle();


            if (currentEval >= beta)
            {
                return beta;
            }
            
            if (currentEval > alpha)
            {
                alpha = currentEval;
            }
        }

        return alpha;
    }

    int searchCaptures(int alpha, int beta, bool whiteToMove)
    {
        int currentEval = evaluator.evaluate(whiteToMove);

        if (currentEval >= beta)
        {
            return beta;
        }

        if (currentEval > alpha)
        {
            alpha = currentEval;
        }

        int pieceColour;

        if (whiteToMove)
        {
            pieceColour = Piece.white;
        }
        else
        {
            pieceColour = Piece.black;
        }

        List<Board.move> moves = mainBoard.calculateAllMoves(pieceColour, true);

        foreach (Board.move move in moves)
        {
            int prev = boardHandeler.storedBoard[move.endIndex];

            boardHandeler.makeMoveMove(move);

            currentEval = -searchCaptures(-beta, -alpha, !whiteToMove);

            boardHandeler.unmakeMove(move, prev);
            mainBoard.revertCastle();

            if (currentEval >= beta)
            {
                return beta;
            }

            if (currentEval > alpha)
            {
                alpha = currentEval;
            }
        }

        return alpha;
    }

    public void changeSearchCaptures(bool value)
    {
        searchCaps = value;
    }

    public void changeMaxMoveTime(float value)
    {
        maxMoveTime = value;
    }
}
