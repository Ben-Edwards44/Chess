using UnityEngine;

public class drawPieces : MonoBehaviour
{
    public GameObject wPawn;
    public GameObject wKnight;
    public GameObject wBishop;
    public GameObject wRook;
    public GameObject wQueen;
    public GameObject wKing;

    public GameObject bPawn;
    public GameObject bKnight;
    public GameObject bBishop;
    public GameObject bRook;
    public GameObject bQueen;
    public GameObject bKing;


    private void Start()
    {
        displayPieces();
    }


    GameObject[] pieceValues()
    {
        GameObject[] pieceValues = new GameObject[12];

        pieceValues[0] = wPawn;
        pieceValues[1] = wKnight;
        pieceValues[2] = wBishop;
        pieceValues[3] = wRook;
        pieceValues[4] = wQueen;
        pieceValues[5] = wKing;
        pieceValues[6] = bPawn;
        pieceValues[7] = bKnight;
        pieceValues[8] = bBishop;
        pieceValues[9] = bRook;
        pieceValues[10] = bQueen;
        pieceValues[11] = bKing;

        return pieceValues;
    }


    void destroyChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }


    public void displayPieces()
    {
        destroyChildren();

        int[] board = boardHandeler.storedBoard;
        GameObject[] values = pieceValues();

        for (int i = 0; i < 8; i++)
        {
            for (int x = 0; x < 8; x++)
            {
                int index = i * 8 + x;
                int currentPiece = board[index];

                if (currentPiece > 0)
                {
                    Vector2 position = new Vector2(i, x);
                    GameObject piece = Instantiate(values[currentPiece - 1], this.transform);

                    piece.transform.position = position;
                    piece.SetActive(true);
                }
            }
        }
    }
}
