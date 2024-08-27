using UnityEngine;

public class Tilemaker : MonoBehaviour{
    public GameObject cellPrefab;
    private float cellSize = 1f;

    public uint rows;
    public uint columns;
    public uint maxBombs;
    private uint currentBombs = 0;

    public GameObject[,] cellGrid { get; private set; }

    void Awake(){
        cellGrid = new GameObject[rows, columns];

        for(int i = 0; i < rows; i++){
            for(int j = 0; j < columns; j++){
                Vector2 position = new Vector2(i * cellSize, j * cellSize);
                cellGrid[i,j] = Instantiate(cellPrefab, position, Quaternion.identity, transform);
            }
        }

        //position the object to the upper right of the screen
        transform.position = new Vector3(-12f, -12f, transform.position.z);
        RandomizedBomb();
    }

    void RandomizedBomb(){
        while (currentBombs != maxBombs){
            int randomRow = Mathf.FloorToInt(Random.Range(0, rows));
            int randomColumn = Mathf.FloorToInt(Random.Range(0, columns));

            Cell randomCell = cellGrid[randomRow, randomColumn].GetComponent<Cell>();
            if(BombRulesCheck(randomCell)){
                MakeBomb(randomCell);
                currentBombs++;
            } 
        }
    }

    void MakeBomb(Cell cellScript){
        cellScript.isBomb = true;
        cellScript.spriteRenderer.color = Color.red;
    }

    bool BombRulesCheck(Cell cellScript){
        return cellScript.surroundingBombs <= 3;
    }

    public (uint x, uint y)[] Corners(){
        return new (uint x, uint y)[]{
            (0,0),
            (0, columns - 1),
            (rows - 1, columns - 1),
            (rows - 1, 0)
        };
    }
}