using UnityEngine;

public class Evaluation : MonoBehaviour
{
    private int[] pieceValues = new int[6] { 100, 300, 300, 500, 900, 5000 };


    private int[] pawnPos = new int[64]
    {
        0,  0,  0,  0,  0,  0,  0,  0 ,
       50, 50, 50, 50, 50, 50, 50, 50 ,
       10, 10, 20, 30, 30, 20, 10, 10 ,
        5,  5, 10, 25, 25, 10,  5,  5 ,
        0,  0,  0, 20, 20,  0,  0,  0 ,
        5, -5,-10,  0,  0,-10, -5,  5 ,
        5, 10, 10,-20,-20, 10, 10,  5 ,
        0,  0,  0,  0,  0,  0,  0,  0 
    };

    private int[] knightPos = new int[64]
    {
        -50,-40,-30,-30,-30,-30,-40,-50 ,
        -40,-20,  0,  0,  0,  0,-20,-40 ,
        -30,  0, 10, 15, 15, 10,  0,-30 ,
        -30,  5, 15, 20, 20, 15,  5,-30 ,
        -30,  0, 15, 20, 20, 15,  0,-30 ,
        -30,  5, 10, 15, 15, 10,  5,-30 ,
        -40,-20,  0,  5,  5,  0,-20,-40 ,
        -50,-40,-30,-30,-30,-30,-40,-50 
    };

    private int[] bishopPos = new int[64]
    {
        -20,-10,-10,-10,-10,-10,-10,-20 ,
        -10,  0,  0,  0,  0,  0,  0,-10 ,
        -10,  0,  5, 10, 10,  5,  0,-10 ,
        -10,  5,  5, 10, 10,  5,  5,-10 ,
        -10,  0, 10, 10, 10, 10,  0,-10 ,
        -10, 10, 10, 10, 10, 10, 10,-10 ,
        -10,  5,  0,  0,  0,  0,  5,-10 ,
        -20,-10,-10,-10,-10,-10,-10,-20 
    };

    private int[] rookPos = new int[64]
    {
          0,  0,  0,  0,  0,  0,  0,  0 ,
          5, 10, 10, 10, 10, 10, 10,  5 ,
         -5,  0,  0,  0,  0,  0,  0, -5 ,
         -5,  0,  0,  0,  0,  0,  0, -5 ,
         -5,  0,  0,  0,  0,  0,  0, -5 ,
         -5,  0,  0,  0,  0,  0,  0, -5 ,
         -5,  0,  0,  0,  0,  0,  0, -5 ,
          0,  0,  0,  5,  5,  0,  0,  0 
    };

    private int[] queenPos = new int[64]
    {
        -20,-10,-10, -5, -5,-10,-10,-20 ,
        -10,  0,  0,  0,  0,  0,  0,-10 ,
        -10,  0,  5,  5,  5,  5,  0,-10 ,
         -5,  0,  5,  5,  5,  5,  0, -5 ,
          0,  0,  5,  5,  5,  5,  0, -5 ,
        -10,  5,  5,  5,  5,  5,  0,-10 ,
        -10,  0,  5,  0,  0,  0,  0,-10 ,
        -20,-10,-10, -5, -5,-10,-10,-20 
    };

    private int[] kingPos = new int[64]
    {
        -30,-40,-40,-50,-50,-40,-40,-30,
        -30,-40,-40,-50,-50,-40,-40,-30,
        -30,-40,-40,-50,-50,-40,-40,-30,
        -30,-40,-40,-50,-50,-40,-40,-30,
        -20,-30,-30,-40,-40,-30,-30,-20,
        -10,-20,-20,-20,-20,-20,-20,-10,
         20, 20,  0,  0,  0,  0, 20, 20,
         20, 30, 10,  0,  0, 10, 30, 20
    };


    private int[] pawnIndexes = new int[8] { -9, -8, -7, 1, 9, 8, 7, -1 };


    public int evaluate(bool whiteToMove)
    {
        int total = 0;

        for (int i = 0; i < 64; i++) 
        {
            int square = boardHandeler.storedBoard[i];

            if (square != 0)
            {
                if (Piece.pieceColour(square) == Piece.white)
                {
                    total += evaluatePiece(square, i, false);
                }
                else
                {
                    total -= evaluatePiece(square - Piece.black, i, true);
                }
            }
        }

        if (whiteToMove)
        {
            return total;
        }
        else
        {
            return -total;
        }
    }


    private int pawnStructure(int index, bool isWhite)
    {
        int total = 0;

        for (int i = 3; i < 8; i += 4)
        {
            int newIndex = index + pawnIndexes[i];

            if (0 <= newIndex && newIndex < 64)
            {
                int square = boardHandeler.storedBoard[newIndex];
                bool squareIsWhite = Piece.pieceColour(square) == Piece.white;

                if (square != 0 && Piece.pieceType(square) == Piece.pawn && squareIsWhite == isWhite)
                {
                    total--;
                }
            }   
        }

        for (int i = 0; i < 7; i += 2)
        {
            int newIndex = index + pawnIndexes[i];

            if (0 <= newIndex && newIndex < 64)
            {
                int square = boardHandeler.storedBoard[newIndex];
                bool squareIsWhite = Piece.pieceColour(square) == Piece.white;

                if (square != 0 && Piece.pieceType(square) == Piece.pawn && squareIsWhite == isWhite)
                {
                    total++;
                }
            }
        }

        return total;
    }


    private int evaluatePiece(int piece, int index, bool isBlack)
    {
        int value = pieceValues[piece - 1];
        int[] posValues = new int[64];

        if (piece == Piece.pawn)
        {
            posValues = pawnPos;
        }
        else if (piece == Piece.knight)
        {
            posValues = knightPos;
        }
        else if (piece == Piece.bishop)
        {
            posValues = bishopPos;
        }
        else if (piece == Piece.rook)
        {
            posValues = rookPos;
        }
        else if (piece == Piece.queen)
        {
            posValues = queenPos;
        }
        else if (piece == Piece.king)
        {
            posValues = kingPos;
        }

        if (!isBlack)
        {
            System.Array.Reverse(posValues);
        }

        int x = (int)index / 8;
        int y = index % 8;

        int posValue = posValues[y * 8 + x];

        if (!isBlack)
        {
            System.Array.Reverse(posValues);
        }

        if (piece == Piece.pawn)
        {
            value += pawnStructure(index, !isBlack);
        }

        return posValue + value;
    }
}
