using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class movementHandeler : MonoBehaviour
{
    private int moveIndex;

    private int selectedX = -1;
    private int selectedY = -1;

    private drawPieces pieceDrawer;
    private Board mainBoard;
    private boardSquares boardSquares;
    private minimax minimax;
    private initialMoves initialMoves;
    private gameOverHandeler gameOverHandeler;

    private bool whiteToMove = true;
    private bool initial = true;

    private Board.move prevPlayerMove;

    public Text depthText;
    public Text moveText;


    private void Start()
    {
        pieceDrawer = FindObjectOfType<drawPieces>();
        mainBoard = FindObjectOfType<Board>();
        boardSquares = FindObjectOfType<boardSquares>();
        minimax = FindObjectOfType<minimax>();
        initialMoves = FindObjectOfType<initialMoves>();
        gameOverHandeler = FindObjectOfType<gameOverHandeler>();
    }


    private void Update()
    {
        if (whiteToMove)
        {
            if (checkLoss())
            {
                depthText.text = "Checkmate!";
                depthText.color = Color.magenta;

                gameOverHandeler.showEndScreen();
            }

            if (Input.GetMouseButtonDown(0))
            {
                playerMove();
            }
        }
        else
        {
            displayComputer();
            whiteToMove = !whiteToMove;
        }
    }


    bool checkLoss()
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

        List<Board.move> playerMoves = mainBoard.calculateAllMoves(pieceColour, false);

        return playerMoves.Count == 0;
    }


    void displayComputer()
    {
        if (initial && initialMoves.gameInBook(prevPlayerMove))
        {
            initialMoves.makeInitialMove(prevPlayerMove);
            depthText.text = "Used book move";
        }
        else
        {
            Board.move move = minimax.calculateBestMove(whiteToMove);
            boardHandeler.makeMoveMove(move);
            initial = false;

            if (move.endIndex != 0 && move.startIndex != 0)
            {
                string moveString = initialMoves.convertFromMove(move);
                moveText.text = "Move: " + moveString;
            }
        }

        mainBoard.updatePrev();
        pieceDrawer.displayPieces();
    }


    void showMoves(int selectedX, int selectedY)
    {
        int pieceIndex = selectedX * 8 + selectedY;
        List<int> moves = getPossibleMoveIndexes(pieceIndex);

        boardSquares.drawPossibleMoves(moves);
    }


    List<int> getPossibleMoveIndexes(int pieceIndex)
    {
        int piece = boardHandeler.storedBoard[pieceIndex];

        int pieceType = Piece.pieceType(piece);
        List<Board.move> moves = new List<Board.move>();

        if (pieceType == Piece.rook)
        {
            moves = mainBoard.rookMoves(pieceIndex, false);
        }
        else if (pieceType == Piece.bishop)
        {
            moves = mainBoard.bishopMoves(pieceIndex, false);
        }
        else if (pieceType == Piece.queen)
        {
            moves = mainBoard.queenMoves(pieceIndex, false);
        }
        else if (pieceType == Piece.knight)
        {
            moves = mainBoard.knightMoves(pieceIndex, false);
        }
        else if (pieceType == Piece.king)
        {
            List<Board.move> castleMoves = mainBoard.castleMoves(whiteToMove);
            moves = mainBoard.kingMoves(pieceIndex, false);

            foreach (Board.move move in castleMoves)
            {
                moves.Add(move);
            }
        }
        else if (pieceType == Piece.pawn)
        {
            moves = mainBoard.pawnMoves(pieceIndex, false);
        }

        moves = mainBoard.filterIllegal(moves, Piece.pieceColour(piece));

        List<int> indexes = new List<int>();

        foreach (Board.move i in moves)
        {
            indexes.Add(i.endIndex);
        }

        return indexes;
    }


    List<Board.move> getPossibleMoves(int pieceIndex)
    {
        int piece = boardHandeler.storedBoard[pieceIndex];

        int pieceType = Piece.pieceType(piece);
        List<Board.move> moves = new List<Board.move>();

        if (pieceType == Piece.rook)
        {
            moves = mainBoard.rookMoves(pieceIndex, false);
        }
        else if (pieceType == Piece.bishop)
        {
            moves = mainBoard.bishopMoves(pieceIndex, false);
        }
        else if (pieceType == Piece.queen)
        {
            moves = mainBoard.queenMoves(pieceIndex, false);
        }
        else if (pieceType == Piece.knight)
        {
            moves = mainBoard.knightMoves(pieceIndex, false);
        }
        else if (pieceType == Piece.king)
        {
            List<Board.move> castleMoves = mainBoard.castleMoves(whiteToMove);
            moves = mainBoard.kingMoves(pieceIndex, false);

            foreach (Board.move move in castleMoves)
            {
                moves.Add(move);
            }
        }
        else if (pieceType == Piece.pawn)
        {
            moves = mainBoard.pawnMoves(pieceIndex, false);
        }

        moves = mainBoard.filterIllegal(moves, Piece.pieceColour(piece));

        return moves;
    }


    private void playerMove()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        worldPosition.x += 0.5f;
        worldPosition.y += 0.5f;

        int x = (int)worldPosition.x;
        int y = (int)worldPosition.y;


        if (0 <= x && x < 8 && 0 <= y && y < 8)
        {
            if (selectedX == -1 || selectedY == -1)
            {
                selectedX = x;
                selectedY = y;

                showMoves(selectedX, selectedY);
            }
            else
            {
                List<Board.move> moves = getPossibleMoves(selectedX * 8 + selectedY);

                Board.move move = new Board.move();
                bool found = false;

                foreach (Board.move i in moves)
                {
                    if (i.endIndex == x * 8 + y && i.startIndex == selectedX * 8 + selectedY)
                    {
                        found = true;
                        move = i;
                    }
                }

                if (found)
                {
                    if (Piece.pieceType(boardHandeler.storedBoard[selectedX * 8 + selectedY]) == Piece.king)
                    {
                        if (selectedX - x == 2 || selectedX - x == -2)
                        {
                            playerCastle(x, y);
                        }
                        else
                        {
                            prevPlayerMove = boardHandeler.makeMove(selectedY, selectedX, y, x);
                        }

                        whiteToMove = !whiteToMove;
                    }
                    else
                    {
                        if (initial)
                        {
                            prevPlayerMove = boardHandeler.makeMove(selectedY, selectedX, y, x);
                        }
                        else
                        {
                            boardHandeler.makeMoveMove(move);
                        }

                        whiteToMove = !whiteToMove;
                    }
                }

                pieceDrawer.displayPieces();

                selectedX = -1;
                selectedY = -1;

                boardSquares.destroyChildren();
            }
        }
    }


    void playerCastle(int x, int y)
    {
        List<Board.move> moves = mainBoard.castleMoves(whiteToMove);

        foreach(Board.move move in moves)
        {
            if (move.endIndex == x * 8 + y)
            {
                boardHandeler.makeMoveMove(move);
            }
        }
    }
}
