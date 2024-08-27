using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public bool isBomb = false;
    public bool isRevealed = false;

    private BoxCollider2D boxCollider2D;

    public List<GameObject> neighbors { get; private set; } = new List<GameObject>();

    public int surroundingBombs
    {
        get
        {
            int bombCount = 0;
            foreach (GameObject cell in neighbors){
                Cell cellScript = cell.GetComponent<Cell>();
                if(cellScript.isBomb){
                    bombCount++;
                }
            }

            return bombCount;
        }

        private set {}
    }
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Start(){
        UpdateNeighbors();
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(!GameManager.isStart){
                Reveal();
            }else{
                StartCheck();
            }
        }
    }

    void StartCheck(){
        if(isBomb){
            BombSwap(this, true);
        }else{
            if(surroundingBombs == 0){
                Reveal();
            }else{
                foreach(GameObject cell in neighbors){
                    Cell neighbor = cell.GetComponent<Cell>();
                    if(neighbor.isBomb){
                        BombSwap(neighbor, false);
                    }
                }
            }
        }

        GameManager.isStart = false;
    }

    void BombSwap(Cell cellToSwap, bool bombMode){
        Tilemaker tilemaker = FindObjectOfType<Tilemaker>();

        foreach(var tuple in tilemaker.Corners()){
            Cell cornerCell = tilemaker.cellGrid[tuple.x, tuple.y].GetComponent<Cell>();
            if(!cornerCell.isBomb){
                //swap tile position
                Vector2 thisPosition = cellToSwap.gameObject.transform.position;
                cellToSwap.gameObject.transform.position = cornerCell.gameObject.transform.position;
                cornerCell.gameObject.transform.position = thisPosition;

                //Bomb mode is if the first revealed cell is a bomb
                if(bombMode){
                    cornerCell.Reveal();
                    break;
                }else{
                    Reveal();
                }
            }
        }
    }

    void Reveal()
    {
        UpdateNeighbors();
        isRevealed = true;
        spriteRenderer.color = Color.blue;

        //check surrounding 0 cells
        if(surroundingBombs == 0){
            foreach(GameObject cell in neighbors){
                Cell neighbor = cell.GetComponent<Cell>();
                if(neighbor != this && !neighbor.isRevealed){
                    neighbor.Reveal();
                }
            }
        }
    }

    void UpdateNeighbors(){
        neighbors.Clear();
        neighbors = Neighbors();
    }

    List<GameObject> Neighbors()
    {
        int layer = LayerMask.GetMask("Tile");

        RaycastHit2D[] rays = Physics2D.BoxCastAll(boxCollider2D.bounds.center, boxCollider2D.size, 0f, Vector2.zero, Mathf.Infinity, layer);

        return rays.Where(hit => hit.collider.CompareTag("Tile")).Select(hit => hit.collider.gameObject).ToList();
    }
}
