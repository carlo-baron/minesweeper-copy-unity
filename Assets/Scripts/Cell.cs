using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public bool isBomb = false;
    public bool isRevealed = false;

    private BoxCollider2D boxCollider2D;

    // public int surroundingBombs
    // {
    //     get
    //     {
    //         int bombCount = 0;
    //         foreach (GameObject cell in Neighbors()){
    //             Cell cellScript = cell.GetComponent<Cell>();
    //             if(cellScript.isBomb){
    //                 bombCount++;
    //             }
    //         }

    //         return bombCount;
    //     }

    //     private set {}
    // }

    public int count;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        count = SurroundingBombs();
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!GameManager.isStart)
            {
                Reveal();
            }
            else
            {
                StartCheck();
                Reveal();
            }
        }
    }

    void StartCheck()
    {
        if (SurroundingBombs() == 0)
        {
            Reveal();
        }
        else
        {
            foreach (GameObject cell in Neighbors())
            {
                Cell neighbor = cell.GetComponent<Cell>();
                if (neighbor.isBomb)
                {
                    BombSwap(neighbor);
                }
            }
        }

        GameManager.isStart = false;
    }

    void BombSwap(Cell cellToSwap)
    {
        Tilemaker tilemaker = FindObjectOfType<Tilemaker>();

        foreach (var tuple in tilemaker.Corners())
        {
            Cell cornerCell = tilemaker.cellGrid[tuple.x, tuple.y].GetComponent<Cell>();
            if (!cornerCell.isBomb)
            {
                //swap tile position
                Vector2 thisPosition = cellToSwap.gameObject.transform.position;
                cellToSwap.gameObject.transform.position = cornerCell.gameObject.transform.position;
                cornerCell.gameObject.transform.position = thisPosition;

                UpdateNeighborBombCounts(cornerCell);
                UpdateNeighborBombCounts(cellToSwap);
            }
        }
    }

    void UpdateNeighborBombCounts(Cell cell)
    {
        cell.count = cell.SurroundingBombs();
        print(cell.count);
    }

    void Reveal()
    {
        isRevealed = true;
        spriteRenderer.color = Color.blue;

        count = SurroundingBombs();

        //check surrounding 0 cells
        if (count == 0)
        {
            foreach (GameObject cell in Neighbors())
            {
                Cell neighbor = cell.GetComponent<Cell>();
                if (neighbor != this && !neighbor.isRevealed)
                {
                    neighbor.Reveal();
                }
            }
        }
    }

    public int SurroundingBombs()
    {
        int bombCount = 0;
        foreach (GameObject cell in Neighbors())
        {
            Cell cellScript = cell.GetComponent<Cell>();
            if (cellScript.isBomb)
            {
                bombCount++;
            }
        }

        return bombCount;
    }

    List<GameObject> Neighbors()
    {
        int layer = LayerMask.GetMask("Tile");

        RaycastHit2D[] rays = Physics2D.BoxCastAll(boxCollider2D.bounds.center, boxCollider2D.size, 0f, Vector2.zero, Mathf.Infinity, layer);

        return rays.Where(hit => hit.collider.CompareTag("Tile")).Select(hit => hit.collider.gameObject).ToList();
    }
}
