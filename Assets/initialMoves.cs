using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class initialMoves : MonoBehaviour
{
    private char[] files = new char[8] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
    private string[] pieces = new string[6] {"", "N", "B", "R", "Q", "K"};

    private List<string> currentPGN = new List<string>();

    private Board mainBoard;

    public Text moveText;


    private void Start()
    {
        mainBoard = FindObjectOfType<Board>();
    }


    public void makeInitialMove(Board.move newMove)
    {
        string PGN = convertFromMove(newMove);

        currentPGN.Add(PGN);

        List<string> possibleGames = searchList();
        string chosenGame = possibleGames[Random.Range(0, possibleGames.Count)];

        string[] moves = chosenGame.Split(' ');

        Board.move move = convertToMove(moves[currentPGN.Count]);

        boardHandeler.makeMoveMove(move);

        string newPGN = convertFromMove(move);

        currentPGN.Add(newPGN);

        moveText.text = "Move: " + newPGN;
    }


    public bool gameInBook(Board.move newMove)
    {
        string PGN = convertFromMove(newMove);

        currentPGN.Add(PGN);

        List<string> possibleGames = searchList();

        currentPGN.Remove(PGN);

        return possibleGames.Count > 0;
    }


    private List<string> searchList()
    {
        string data = readFile();
        string[] games = data.Split("\n");

        List<string> possibleGames = new List<string>();

        foreach (string i in games)
        {
            string[] game = i.Split(' ');

            if (checkEqual(game))
            {
                possibleGames.Add(i);
            }
        }

        return possibleGames;
    }


    private bool checkEqual(string[] currentGame)
    {
        for (int i = 0; i < currentPGN.Count; i++)
        {
            string s1 = currentPGN[i];
            string s2 = currentGame[i];

            if (s1 != s2) 
            { 
                return false;
            }
        }

        return true;
    }


    public string convertFromMove(Board.move move)
    {
        if (move.kingCastle)
        {
            return "O-O";
        }
        else if (move.queenCastle)
        {
            return "O-O-O";
        }

        int x = move.endIndex / 8;
        int y = move.endIndex % 8 + 1;

        char file = files[x];
        string rank = y.ToString();

        int piece = boardHandeler.storedBoard[move.endIndex];

        string pieceString = pieces[Piece.pieceType(piece) - 1];

        return pieceString + file + rank;
    }


    private Board.move convertToMove(string PGN)
    {
        if (PGN == "O-O")
        {
            if (currentPGN.Count % 2 == 0)
                return new Board.move(32, 48, false, true);
            else
                return new Board.move(39, 55, false, true);
        }
        else if (PGN == "O-O-O")
        {
            if (currentPGN.Count % 2 == 0)
                return new Board.move(32, 16, false, false, true);
            else
                return new Board.move(39, 23, false, false, true);
        }

        int pieceColour;
        if (currentPGN.Count % 2 == 0)
        {
            pieceColour = Piece.white;
        }
        else
        {
            pieceColour = Piece.black;
        }


        int pieceNum = 0;
        if (PGN.Length > 2)
        {
            char piece = PGN[0];

            for (int i = 0; i < 6; i++)
            {
                if (pieces[i] == piece.ToString())
                {
                    pieceNum = i + 1;
                }
            }
        }
        else
        {
            pieceNum = 1;
            PGN = "X" + PGN;
        }

        int x = 0;
        for (int i = 0; i < 8; i++)
        {
            if (files[i] == PGN[1])
            {
                x = i;
            }
        }

        char rank = PGN[2];
        int y = (int) char.GetNumericValue(rank);
        y--;

        int endIndex = x * 8 + y;

        List<Board.move> possibleMoves = mainBoard.calculateAllMoves(pieceColour, false);
        foreach (Board.move move in possibleMoves)
        {
            int piece = boardHandeler.storedBoard[move.startIndex];

            if (Piece.pieceType(piece) == pieceNum && move.endIndex == endIndex)
            {
                return move;
            }
        }

        return new Board.move();
    }


    private static string readFile()
    {
        string path = Application.dataPath + "/Resources/chessGames.txt";
        string data = File.ReadAllText(path);

        return data;
    }
}
