using System.Collections.Generic;
using UnityEngine;

public class boardHandeler : MonoBehaviour
{
    public static int[] storedBoard = new int[64];


    private void Awake()
    {
        string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        convertFromFen(fen);
    }


    public static Board.move makeMove(int x, int y, int toX, int toY)
    {
        int value = storedBoard[y * 8 + x];

        storedBoard[y * 8 + x] = 0;
        storedBoard[toY * 8 + toX] = value;

        return new Board.move(y * 8 + x, toY * 8 + toX);
    }


    public static void makeMoveMove(Board.move move)
    {
        int value = storedBoard[move.startIndex];

        if (move.promotion)
        {
            value = Piece.queen + Piece.pieceColour(value);
        }

        if (move.kingCastle || move.queenCastle)
        {
            storedBoard[move.startIndex] = 0;
            storedBoard[move.endIndex] = Piece.king + Piece.pieceColour(value);
            storedBoard[(move.startIndex + move.endIndex) / 2] = Piece.rook + Piece.pieceColour(value);

            if (Piece.pieceColour(value) == Piece.white)
            {
                if (move.kingCastle)
                {
                    storedBoard[56] = 0;
                }
                else
                {
                    storedBoard[0] = 0;
                }
            }
            else
            {
                if (move.kingCastle)
                {
                    storedBoard[63] = 0;
                }
                else
                {
                    storedBoard[7] = 0;
                }
            }
        }
        else
        {
            storedBoard[move.startIndex] = 0;
            storedBoard[move.endIndex] = value;
        }
    }


    public static void unmakeMove(Board.move move, int prev) 
    {
        int value = storedBoard[move.endIndex];

        if (move.promotion)
        {
            value = Piece.pawn + Piece.pieceColour(value);
        }

        if (move.kingCastle || move.queenCastle)
        {
            storedBoard[move.startIndex] = Piece.king + Piece.pieceColour(value);
            storedBoard[move.endIndex] = 0;
            storedBoard[(move.startIndex + move.endIndex) / 2] = 0;

            if (Piece.pieceColour(value) == Piece.white)
            {
                if (move.kingCastle)
                {
                    storedBoard[56] = Piece.rook + Piece.white;
                }
                else
                {
                    storedBoard[0] = Piece.rook + Piece.white;
                }
            }
            else
            {
                if (move.kingCastle)
                {
                    storedBoard[63] = Piece.rook + Piece.black;
                }
                else
                {
                    storedBoard[7] = Piece.rook + Piece.black;
                }
            }
        }

        storedBoard[move.endIndex] = prev;
        storedBoard[move.startIndex] = value;
    }


    void convertFromFen(string fen)
    {
        Dictionary<char, int> pieceValues = new Dictionary<char, int>()
        {
            {'p', Piece.pawn},
            {'n', Piece.knight},
            {'b', Piece.bishop},
            {'r', Piece.rook},
            {'q', Piece.queen},
            {'k', Piece.king}
        };

        string[] new_fen = fen.Split('/');

        int index1 = 0;
        int index2 = 0;

        for (int i = 0; i < 8; i++)
        {
            index1 = 7 - i;
            index2 = 0;

            string row = new_fen[i];
            int x = 0;
            while (x < row.Length)
            {
                char square = row[x];

                if (char.IsDigit(square))
                {
                    for (int _ = 0; _ < int.Parse(square.ToString()); _++) 
                    {
                        storedBoard[index2 * 8 + index1] = 0;
                        index2++;
                    }
                }
                else
                {
                    int add = 0;

                    if (char.IsLower(square))
                    {
                        add = Piece.black;
                    }
                    else
                    {
                        add = Piece.white;
                    }

                    int value = pieceValues[char.ToLower(square)];

                    storedBoard[index2 * 8 + index1] = value + add;

                    index2++;
                }

                x++;
            }
        }
    }
}
