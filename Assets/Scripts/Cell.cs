using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    }
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
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
            Tilemaker tilemaker = FindObjectOfType<Tilemaker>();

            foreach(var tuple in tilemaker.Corners()){
                Cell cornerCell = tilemaker.cellGrid[tuple.x, tuple.y].GetComponent<Cell>();
                if(!cornerCell.isBomb){
                    Vector2 thisPosition = transform.position;
                    transform.position = cornerCell.gameObject.transform.position;
                    cornerCell.gameObject.transform.position = thisPosition;
                    cornerCell.Reveal();
                    break;
                }
            } 
        }else{
            Reveal();
        }

        GameManager.isStart = false;
    }

    void Reveal()
    {
        isRevealed = true;
        spriteRenderer.color = Color.blue;

        //check surrounding 0 cells
    }

    List<GameObject> Neighbors()
    {
        LayerMask layer = gameObject.layer;

        RaycastHit2D[] rays = Physics2D.BoxCastAll(boxCollider2D.bounds.center, boxCollider2D.size, 0f, Vector2.zero, Mathf.Infinity, layer);

        return rays.Where(hit => hit.collider.CompareTag("Tile")).Select(hit => hit.collider.gameObject).ToList();
    }
}
