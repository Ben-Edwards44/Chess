using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private readonly int[] directions = new int[8] { -9, -8, -7, 1, 9, 8, 7, -1 };
    private readonly int[,] knightDirections = new int[8, 2] { { 2, 1 },
                                                               { 1, 2 },
                                                               { -1, 2 },
                                                               { 2, -1 },
                                                               { 1, -2 },
                                                               { -2, 1 },
                                                               { -2, -1 },
                                                               { -1, -2 } };

    private int[,] distToEdge;

    private checkmate mate;

    private bool canCastleKingW;
    private bool canCastleQueenW;
    private bool canCastleKingB;
    private bool canCastleQueenB;

    private bool prevKingW;
    private bool prevQueenW;
    private bool prevKingB;
    private bool prevQueenB;


    private void Start()
    {
        calculateDistToEdge();
        mate = FindObjectOfType<checkmate>();
    }


    public struct move
    {
        public readonly int startIndex;
        public readonly int endIndex;
        public readonly bool promotion;
        public readonly bool kingCastle;
        public readonly bool queenCastle;

        public move(int startIndex, int endIndex, bool promotion = false, bool kingCastle = false, bool queenCastle = false)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.promotion = promotion;
            this.kingCastle = kingCastle;
            this.queenCastle = queenCastle;
        }
    }


    int min(int a, int b)
    {
        return a < b ? a : b;
    }


    int max(int a, int b)
    {
        return a > b ? a : b;
    }


    private void calculateDistToEdge()
    {
        distToEdge = new int[64, 8];

        for (int i = 0; i < 8; i++)
        {
            for (int x = 0; x < 8; x++)
            {
                int up = 7 - x;
                int down = x;
                int left = i;
                int right = 7 - i;

                int[] directions = new int[8]
                {
                    min(down, left),
                    down,
                    min(down, right),
                    right,
                    min(up, right),
                    up,
                    min(up, left),
                    left
                };

                for (int j = 0; j < 8; j++)
                {
                    distToEdge[x * 8 + i, j] = directions[j];
                }
            }
        }
    }


    List<move> getPossibleMoves(int pieceIndex, int piece, bool onlyCaptures)
    {
        int pieceType = Piece.pieceType(piece);
        List<Board.move> moves = new List<Board.move>();

        if (pieceType == Piece.rook)
        {
            moves = rookMoves(pieceIndex, onlyCaptures);
        }
        else if (pieceType == Piece.bishop)
        {
            moves = bishopMoves(pieceIndex, onlyCaptures);
        }
        else if (pieceType == Piece.queen)
        {
            moves = queenMoves(pieceIndex, onlyCaptures);
        }
        else if (pieceType == Piece.knight)
        {
            moves = knightMoves(pieceIndex, onlyCaptures);
        }
        else if (pieceType == Piece.king)
        {
            moves = kingMoves(pieceIndex, onlyCaptures);
        }
        else if (pieceType == Piece.pawn)
        {
            moves = pawnMoves(pieceIndex, onlyCaptures);
        }

        return moves;
    }


    public List<move> calculateAllMoves(int pieceColour, bool onlyCaptures, bool filter = true)
    {
        List<move> moves = new List<move>();

        for (int i = 0; i < 64; i++)
        {
            int square = boardHandeler.storedBoard[i];

            if (!filter && Piece.pieceType(square) == Piece.king) continue;

            if (square != 0 && Piece.pieceColour(square) == pieceColour)
            {
                List<move> pieceMoves = getPossibleMoves(i, square, onlyCaptures);

                foreach (move x in pieceMoves)
                {
                    moves.Add(x);
                }
            }
        }

        if (!onlyCaptures)
        {
            List<move> castles = castleMoves(pieceColour == Piece.white);

            foreach (move move in castles)
            {
                moves.Add(move);
            }
        }

        if (filter)
        {
            moves = filterIllegal(moves, pieceColour);
        }

        return moves;
    }


    public List<move> filterIllegal(List<move> moves, int pieceColour) 
    { 
        List<move> legalMoves = new List<move>();

        foreach (move move in moves)
        {
            int prev = boardHandeler.storedBoard[move.endIndex];
            boardHandeler.makeMoveMove(move);

            if (!mate.kingInCheck(pieceColour))
            {
                if (Piece.pieceType(boardHandeler.storedBoard[move.endIndex]) == Piece.king)
                {
                    if (kingCheck())
                    {
                        legalMoves.Add(move);
                    }
                }
                else
                {
                    legalMoves.Add(move);
                }
            }

            boardHandeler.unmakeMove(move, prev);
        }

        return legalMoves;
    }


    private bool kingCheck()
    {
        int wKingPos = 0;
        int bKingPos = 0;

        for (int i = 0; i < 64; i++)
        {
            int square = boardHandeler.storedBoard[i];

            if (square != 0 && Piece.pieceType(square) == Piece.king)
            {
                if (Piece.pieceColour(square) == Piece.white)
                {
                    wKingPos = i;
                }
                else
                {
                    bKingPos = i;
                }
            }
        }

        int distX = ((int)wKingPos / 8) - ((int)bKingPos / 8);
        int distY = (wKingPos % 8) - (bKingPos % 8);

        return !(Mathf.Abs(distX) <= 1 && Mathf.Abs(distY) <= 1);
    }


    public void revertCastle()
    {
        canCastleKingW = prevKingW;
        canCastleKingB = prevKingB;
        canCastleQueenB = prevQueenB;
        canCastleQueenW = prevQueenW;
    }


    public void updatePrev()
    {
        prevKingW = canCastleKingW;
        prevKingB = canCastleKingB;
        prevQueenB = canCastleQueenB;
        prevQueenW = canCastleQueenW;
    }


    private void updateCastle()
    {
        canCastleKingB = true;
        canCastleQueenB = true;
        canCastleKingW = true;
        canCastleQueenW = true;

        if (boardHandeler.storedBoard[32] != Piece.king + Piece.white)
        {
            canCastleKingW = false;
            canCastleQueenW = false;
        }
        if (boardHandeler.storedBoard[39] != Piece.king + Piece.black)
        {
            canCastleKingB = false;
            canCastleQueenB = false;
        }

        if (boardHandeler.storedBoard[0] != Piece.rook + Piece.white)
        {
            canCastleQueenW = false;
        }
        if (boardHandeler.storedBoard[56] != Piece.rook + Piece.white)
        {
            canCastleKingW = false;
        }

        if (boardHandeler.storedBoard[7] != Piece.rook + Piece.black)
        {
            canCastleQueenB = false;
        }
        if (boardHandeler.storedBoard[63] != Piece.rook + Piece.black)
        {
            canCastleKingB = false;
        }

        if (boardHandeler.storedBoard[24] != 0)
        {
            canCastleQueenW = false;
        }
        if (boardHandeler.storedBoard[40] != 0)
        {
            canCastleKingW = false;
        }
        if (boardHandeler.storedBoard[31] != 0)
        {
            canCastleQueenB = false;
        }
        if (boardHandeler.storedBoard[47] != 0)
        {
            canCastleKingB = false;
        }

        if (boardHandeler.storedBoard[16] != 0)
        {
            canCastleQueenW = false;
        }
        if (boardHandeler.storedBoard[48] != 0)
        {
            canCastleKingW = false;
        }
        if (boardHandeler.storedBoard[23] != 0)
        {
            canCastleQueenB = false;
        }
        if (boardHandeler.storedBoard[55] != 0)
        {
            canCastleKingB = false;
        }

        if (mate.kingInCheck(Piece.white, true))
        {
            canCastleQueenW = false;
            canCastleKingW = false;
        }
        if (mate.kingInCheck(Piece.black, true))
        {
            canCastleQueenB = false;
            canCastleKingB = false;
        }

        mate.kingInCheck(Piece.black, true);
    }


    public List<move> castleMoves(bool whiteMove)
    {
        updateCastle();

        List<move> moves = new List<move>();

        if (whiteMove)
        {
            if (canCastleKingW)
            {
                moves.Add(new move(32, 48, false, true));
            }
            if (canCastleQueenW)
            {
                moves.Add(new move(32, 16, false, false, true));
            }
        }
        else
        {
            if (canCastleKingB)
            {
                moves.Add(new move(39, 55, false, true));
            }
            if (canCastleQueenB)
            {
                moves.Add(new move(39, 23, false, false, true));
            }
        }

        return moves;
    }


    public List<move> rookMoves(int startIndex, bool onlyCaptures)
    {
        List<move> moves = new List<move>();
        int colour = Piece.pieceColour(boardHandeler.storedBoard[startIndex]);

        for (int i = 1; i < 8; i += 2)
        {
            int direction = directions[i];

            for (int n = 1; n <= distToEdge[startIndex, i]; n++)
            {
                int endIndex = startIndex + direction * n;

                int square = boardHandeler.storedBoard[endIndex];

                if (square == 0)
                {
                    if (!onlyCaptures)
                    {
                        moves.Add(new move(startIndex, endIndex));
                    }
                }
                else
                {
                    if (Piece.pieceColour(square) == colour)
                    {
                        break;
                    }
                    else
                    {
                        moves.Add(new move(startIndex, endIndex));
                        break;
                    }
                }
            }
        }

        return moves;
    }


    public List<move> bishopMoves(int startIndex, bool onlyCaptures)
    {
        List<move> moves = new List<move>();
        int colour = Piece.pieceColour(boardHandeler.storedBoard[startIndex]);

        for (int i = 0; i < 8; i += 2)
        {
            int direction = directions[i];

            for (int n = 1; n <= distToEdge[startIndex, i]; n++)
            {
                int endIndex = startIndex + direction * n;

                int square = boardHandeler.storedBoard[endIndex];

                if (square == 0)
                {
                    if (!onlyCaptures)
                    {
                        moves.Add(new move(startIndex, endIndex));
                    }
                }
                else
                {
                    if (Piece.pieceColour(square) == colour)
                    {
                        break;
                    }
                    else
                    {
                        moves.Add(new move(startIndex, endIndex));
                        break;
                    }
                }
            }
        }

        return moves;
    }


    public List<move> queenMoves(int startIndex, bool onlyCaptures)
    {
        List<move> rook = rookMoves(startIndex, onlyCaptures);
        List<move> bishop = bishopMoves(startIndex, onlyCaptures);

        List<move> moves = new List<move>();


        for (int i = 0; i < rook.Count; i++)
        {
            moves.Add(rook[i]);
        }

        for (int i = 0; i < bishop.Count; i++)
        {
            moves.Add(bishop[i]);
        }

        return moves;
    }


    public List<move> knightMoves(int startIndex, bool onlyCaptures)
    {
        List<move> moves = new List<move>();
        int pieceColour = Piece.pieceColour(boardHandeler.storedBoard[startIndex]);

        int x = (int)startIndex / 8;
        int y = startIndex % 8;

        for (int i = 0; i < 8; i++)
        {
            int newX = x + knightDirections[i, 0];
            int newY = y + knightDirections[i, 1];

            if (newX >= 0 && newX < 8 && newY >= 0 && newY < 8)
            {
                int endIndex = newX * 8 + newY;
                int square = boardHandeler.storedBoard[endIndex];

                if (square == 0)
                {
                    if (!onlyCaptures)
                    {
                        moves.Add(new move(startIndex, endIndex));
                    }
                }
                else if (Piece.pieceColour(square) != pieceColour)
                {
                    moves.Add(new move(startIndex, endIndex));
                }
            }
        }

        return moves;
    }

    public List<move> kingMoves(int startIndex, bool onlyCaptures)
    {
        List<move> moves = new List<move>();
        int colour = Piece.pieceColour(boardHandeler.storedBoard[startIndex]);

        for (int i = 0; i < 8; i++)
        {
            if (distToEdge[startIndex, i] < 1)
            {
                continue;
            }

            int direction = directions[i];
            int endIndex = startIndex + direction;
            int square = boardHandeler.storedBoard[endIndex];

            if (mate.validKingMove(endIndex, colour))
            {
                if (square == 0)
                {
                    if (!onlyCaptures)
                    {
                        moves.Add(new move(startIndex, endIndex));
                    }
                }
                else
                {
                    if (Piece.pieceColour(square) != colour)
                    {
                        moves.Add(new move(startIndex, endIndex));
                    }
                }
            }
        }

        return moves;
    }


    public List<move> pawnMoves(int startIndex, bool onlyCaptures)
    {
        List<move> moves = new List<move>();

        int piece = boardHandeler.storedBoard[startIndex];
        int pieceColour = Piece.pieceColour(piece);

        int multiplier = 1;

        if (pieceColour == Piece.black)
        {
            multiplier = -1;
        }

        int x = (int)startIndex / 8;
        int y = startIndex % 8;

        int endIndex = startIndex + multiplier;

        if (y == 1 && pieceColour == Piece.black || y == 6 && pieceColour == Piece.white)
        {
            int add;

            if (pieceColour == Piece.black)
            {
                add = -1;
            }
            else
            {
                add = 1;
            }

            if (boardHandeler.storedBoard[startIndex + add] == 0 && !onlyCaptures)
            {
                moves.Add(new move(startIndex, startIndex + add, true));
            }
        }
        else if (!onlyCaptures && boardHandeler.storedBoard[endIndex] == 0)
        {
            if (!onlyCaptures)
            {
                moves.Add(new move(startIndex, endIndex));
            }

            if (pieceColour == Piece.white && y == 1 || pieceColour == Piece.black && y == 6)
            {
                int newY = y + multiplier * 2;

                endIndex = x * 8 + newY;

                if (boardHandeler.storedBoard[endIndex] == 0 && !onlyCaptures)
                {
                    moves.Add(new move(startIndex, endIndex));
                }
            }
        }

        int takeY = y + multiplier;
        int takeX;

        for (int i = -1; i < 2; i += 2)
        {
            takeX = x + i;

            if (0 <= takeX && takeX < 8 && 0 <= takeY && takeY < 8)
            {
                endIndex = takeX * 8 + takeY;
                int square = boardHandeler.storedBoard[endIndex];

                if (square != 0 && Piece.pieceColour(square) != pieceColour)
                {
                    moves.Add(new move(startIndex, endIndex));
                }
            }
        }
        
        return moves;
    }
}
