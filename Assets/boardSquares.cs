using System.Collections.Generic;
using UnityEngine;

public class boardSquares : MonoBehaviour
{
    public GameObject lightTile;
    public GameObject darkTile;
    public GameObject moveTile;
    public Transform tempSquares;


    void Start()
    {
        drawBackground();
    }


    void drawBackground()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int x = 0; x < 8; x++)
            {
                GameObject tile;

                if ((i + x) % 2 == 0)
                {
                    tile = darkTile;
                }
                else
                {
                    tile = lightTile;
                }

                drawBox(i, x, tile, this.transform);
            }
        }
    }


    public void destroyChildren()
    {
        foreach (Transform child in tempSquares)
        {
            Destroy(child.gameObject);
        }
    }


    public void drawPossibleMoves(List<int> possibleMoves)
    {
        destroyChildren();

        foreach (int i in possibleMoves)
        {
            int x = (int) i / 8;
            int y = i % 8;

            drawBox(x, y, moveTile, tempSquares);
        }
    }


    void drawBox(int x, int y, GameObject tile, Transform parent)
    {
        Vector2 position = new Vector2(x, y);

        GameObject newTile = Instantiate(tile, parent);

        newTile.SetActive(true);
        newTile.transform.position = position;
    }
}
