using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private TileSO[] tiles;
    [SerializeField] private GameObject[] Cells;
    [SerializeField] private GameObject tile;
    public float animationTime = 0.06f;
    private readonly int gameStartTileCount = 2;
    private int gridSize;
    private bool onAnimated = false;
    private bool onAnimated2 = false;
    private bool isOver;
    public static GameController instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        gridSize = (int)Mathf.Sqrt(Cells.Length);
        NewGame();
    }
    void Update()
    {
        if (!onAnimated&&!onAnimated2&&!isOver)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                Func(0, 1, 1, 1, Vector2Int.down);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Func(1, 0, 1, 1, Vector2Int.left);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                Func(0, gridSize - 2, 1, -1, Vector2Int.up);
            }         
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                Func(gridSize - 2, 0, -1, 1, Vector2Int.right);
            }
        }
    }

    private void Func(int startIndexX, int startIndexY, int increaseX, int increaseY, Vector2Int direction)
    {
        int counter = 0;
        for (int x = startIndexX; x >= 0 && x < gridSize; x += increaseX)
        {
            for (int y = startIndexY; y >= 0 && y < gridSize; y += increaseY)
            {
                Cell cell = GetCellWithCoordinates($"{x},{y}");
                if (cell.isEmpty)
                    continue;
                Cell neighbourCell = GetCellWithCoordinates($"{x + direction.x},{y + direction.y}");
                while (neighbourCell.isEmpty)
                {
                    int targetX = direction.x + neighbourCell.CoordX;
                    int targetY = direction.y + neighbourCell.CoordY;
                    if (targetX >= 0 && targetX < gridSize && targetY >= 0 && targetY < gridSize)
                    {
                        neighbourCell = GetCellWithCoordinates($"{targetX},{targetY}");
                    }
                    else
                        break;
                }
                if (neighbourCell.isEmpty)
                {
                    ChangeTilePlace(cell, neighbourCell);;
                    counter++;
                }
                else //Birleþme 
                {
                    if (cell.tile.prop.Value == neighbourCell.tile.prop.Value&&!neighbourCell.isLocked)
                    {
                        DestroyImmediate(cell.tile.gameObject);
                        int IndexOf = neighbourCell.tile.prop.id;
                        int tileValue=neighbourCell.tile.prop.Value;
                        neighbourCell.tile.prop = tiles[IndexOf + 1];
                        TileAnimate2(neighbourCell.tile.transform); 
                        neighbourCell.isLocked = true;
                        counter++;
                        UIController.Instance.IncreaseScore(tileValue);
                    }
                    else
                    {
                        neighbourCell = GetCellWithCoordinates($"{neighbourCell.CoordX - direction.x},{neighbourCell.CoordY - direction.y}");
                        if(cell!=neighbourCell)
                        {
                            ChangeTilePlace(cell, neighbourCell);
                            counter++;
                        }                    
                    }
                }               
            }
        }
        if(counter>0)StartCoroutine(CreateTile(true));
    }
    
    private void ChangeTilePlace(Cell oldParent, Cell newParent)
    {
        GameObject go = oldParent.tile.gameObject;
        Vector3 startPos = go.transform.position;
        Vector3 targetpos = newParent.GetComponent<RectTransform>().position;
        StartCoroutine(TileAnimate1(startPos, targetpos, go));
        go.transform.SetParent(newParent.transform);
    }
    private IEnumerator TileAnimate1(Vector3 from, Vector3 to, GameObject obj)
    {
        onAnimated = true;
        float elapsed = 0;
        while (elapsed < animationTime)
        {
            if (obj == null)
                break;
            elapsed += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(from, to, elapsed / animationTime);
            yield return null;
        }
        onAnimated = false;
    }
    private void TileAnimate2(Transform animObject)
    {
        onAnimated2 = true;
        animObject.DOScale(1.25f, animationTime).OnComplete(() =>
        {
            animObject.DOScale(1f, animationTime).OnComplete(() =>
            {
                onAnimated2 = false;
            });
        });
    }
    private Cell GetCellWithCoordinates(string coord)
    {
        GameObject go = GameObject.Find(coord);
        if(go!=null)
        return go.GetComponent<Cell>();
        else return null;
    }

    private void GameStartsTile()
    {
        for (int i = 0; i < gameStartTileCount; i++)
        {
            StartCoroutine(CreateTile(false));
        }
    }
    private IEnumerator CreateTile(bool shouldWait)
    {      
        if (shouldWait)
        yield return new WaitForSeconds(animationTime);
        GameObject emptyCell = GetEmptyCell();
        if(emptyCell!=null)
        {
            Tile t = Instantiate(tile, emptyCell.transform).GetComponent<Tile>();
            t.prop = tiles[0];
        }
        if (isGameOver())
        {
            isOver = true;
            UIController.Instance.GameOver();
        }

        foreach (var cell in Cells)
        {
            cell.GetComponent<Cell>().isLocked = false;
        }

    }
    private GameObject GetEmptyCell()
    {
        int randomValue = UnityEngine.Random.Range(0, Cells.Length);
        int initialValue = randomValue;
        while (!Cells[randomValue].GetComponent<Cell>().isEmpty)
        {
            randomValue++;
            if (randomValue >= Cells.Length)
                randomValue = 0;
            if (randomValue == initialValue)
            {              
                return null;
            }
        }
        return Cells[randomValue];
    }
    bool isGameOver()
    {
        Tile[] activeTiles = FindObjectsOfType<Tile>();

        if (activeTiles.Length != Cells.Length)
            return false;
        foreach (var activeTile in activeTiles)
        {
            Cell upCell = GetCellWithCoordinates($"{activeTile.gameObject.GetComponentInParent<Cell>().CoordX},{activeTile.gameObject.GetComponentInParent<Cell>().CoordY - 1}");
            Cell bottomCell = GetCellWithCoordinates($"{activeTile.gameObject.GetComponentInParent<Cell>().CoordX},{activeTile.gameObject.GetComponentInParent<Cell>().CoordY +1}");
            Cell rightCell = GetCellWithCoordinates($"{activeTile.gameObject.GetComponentInParent<Cell>().CoordX+1},{activeTile.gameObject.GetComponentInParent<Cell>().CoordY}");
            Cell leftCell = GetCellWithCoordinates($"{activeTile.gameObject.GetComponentInParent<Cell>().CoordX-1},{activeTile.gameObject.GetComponentInParent<Cell>().CoordY}");

            if (upCell != null && CanMerge(activeTile, upCell.tile))
            {
                return false;
            }

            if (bottomCell != null && CanMerge(activeTile, bottomCell.tile))
            {
                return false;
            }

            if (leftCell != null && CanMerge(activeTile, leftCell.tile))
            {
                return false;
            }

            if (rightCell != null && CanMerge(activeTile, rightCell.tile))
            {
                return false;
            }
        }
        return true;
    }
    private bool CanMerge(Tile a, Tile b)
    {
        return a.prop == b.prop;
    }
    public void NewGame()
    {
        UIController.Instance.SetScore(0);
        UIController.Instance.LoadHighScore();
        Tile[]tiles=FindObjectsOfType<Tile>();
        isOver = false;
        onAnimated = false;
        onAnimated2 = false;
        foreach (Tile tile in tiles)
        {
            DestroyImmediate(tile.gameObject);
        }
        GameStartsTile();
    }
}
