using System.Collections.Generic;
using UnityEngine;

public class checkmate : MonoBehaviour
{
    private List<int> attackingSquares = new List<int>();

    private Board mainBoard;

    private void Start()
    {
        mainBoard = FindObjectOfType<Board>();
    }

    public void slowUpdate(int colour)
    {
        attackingSquares.Clear();

        int pieceColour;
        if (colour == Piece.white)
        {
            pieceColour = Piece.black;
        }
        else
        {
            pieceColour = Piece.white;
        }

        List<Board.move> moves = mainBoard.calculateAllMoves(pieceColour, true, false);

        foreach (Board.move move in moves)
        {
            attackingSquares.Add(move.endIndex);
        }
    }

    public void updateAttacking(List<Board.move> moves)
    {
        attackingSquares.Clear();

        foreach (Board.move move in moves)
        {
            attackingSquares.Add(move.endIndex);
        }
    }

    public bool kingInCheck(int pieceColour, bool update=true)
    {
        if (update)
        {
            slowUpdate(pieceColour);
        }

        int inx = -1;

        for (int index = 0; index < 64; index++)
        {
            int i = boardHandeler.storedBoard[index];

            if (i != 0 && Piece.pieceType(i) == Piece.king && Piece.pieceColour(i) == pieceColour)
            {
                inx = index;
            }
        }

        foreach (int i in attackingSquares)
        {
            if (i == inx) return true;
        }

        return false;
    }

    public bool validKingMove(int index, int colour)
    {
        slowUpdate(colour);

        foreach (int i in attackingSquares)
        {
            if (i == index) return false;
        }

        return true;
    }
}
