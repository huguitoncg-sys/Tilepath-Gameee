using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    [Header("Level")]
    public LevelData level;

    [Header("Tilemaps")]
    public Tilemap floorTilemap;
    public Tilemap wallsTilemap;

    [Header("Tiles")]
    public TileBase blueTile;
    public TileBase redTile;
    public TileBase wallTile;

    public Vector2Int StartPos { get; private set; }

    private bool[,] blocked;
    private bool[,] visited;

    private int openTileCount;
    private int visitedCount;

    public bool IsComplete => visitedCount >= openTileCount;

    private void Start()
    {
        // If player selected a level from the menu, use it.
        if (GameManager.Instance != null && GameManager.Instance.SelectedLevel != null)
        {
            level = GameManager.Instance.SelectedLevel;
        }

        BuildLevel();
    }


    public void BuildLevel()
    {
        if (level == null) { Debug.LogError("No LevelData assigned."); return; }

        floorTilemap.ClearAllTiles();
        wallsTilemap.ClearAllTiles();

        int w = level.Width;
        int h = level.Height;

        blocked = new bool[w, h];
        visited = new bool[w, h];

        openTileCount = 0;
        visitedCount = 0;
        StartPos = new Vector2Int(0, 0);

        // level.rows is top->bottom; Tilemap y is bottom->top
        for (int row = 0; row < h; row++)
        {
            string line = level.rows[row];
            for (int x = 0; x < w; x++)
            {
                char c = line[x];
                int y = (h - 1) - row; // flip y so row 0 is top

                Vector3Int cell = new Vector3Int(x, y, 0);

                if (c == '#')
                {
                    blocked[x, y] = true;
                    wallsTilemap.SetTile(cell, wallTile);
                }
                else
                {
                    // open tile ('.' or 'S')
                    floorTilemap.SetTile(cell, blueTile);
                    openTileCount++;

                    if (c == 'S')
                        StartPos = new Vector2Int(x, y);
                }
            }
        }

        // Count start tile as visited (like your Java version marks start as true)
        Visit(StartPos);
    }

    public bool InBounds(Vector2Int p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < blocked.GetLength(0) && p.y < blocked.GetLength(1);
    }

    public bool CanStep(Vector2Int p)
    {
        return InBounds(p) && !blocked[p.x, p.y] && !visited[p.x, p.y];
    }

    public void Visit(Vector2Int p)
    {
        if (!InBounds(p) || blocked[p.x, p.y] || visited[p.x, p.y]) return;

        visited[p.x, p.y] = true;
        visitedCount++;

        floorTilemap.SetTile(new Vector3Int(p.x, p.y, 0), redTile);
    }

    public void Unvisit(Vector2Int p)
    {
        if (!InBounds(p) || blocked[p.x, p.y] || !visited[p.x, p.y]) return;

        visited[p.x, p.y] = false;
        visitedCount--;

        floorTilemap.SetTile(new Vector3Int(p.x, p.y, 0), blueTile);
    }

    public Vector3 CellToWorldCenter(Vector2Int p)
    {
        // Puts player in the center of the cell
        return floorTilemap.GetCellCenterWorld(new Vector3Int(p.x, p.y, 0));
    }
}
