using UnityEngine;

public class Piece : MonoBehaviour
{
    public static int pawn = 1;
    public static int knight = 2;
    public static int bishop = 3;
    public static int rook = 4;
    public static int queen = 5;
    public static int king = 6;

    public static int white = 0;
    public static int black = 6;


    public static int pieceColour(int piece)
    {
        if (piece < 7)
        {
            return white;
        }
        else
        {
            return black;
        }
    }


    public static int pieceType(int piece)
    {
        return piece - pieceColour(piece);
    }
}
