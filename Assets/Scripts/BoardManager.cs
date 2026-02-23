using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    [Header("Level")]
    public LevelData level;

    [Header("Tilemaps")]
    public Tilemap floorTilemap;
    public Tilemap wallsTilemap;

    // NEW: draw the snake on its own tilemap (set Order in Layer higher than floor)
    public Tilemap snakeTilemap;

    [Header("Board Tiles")]
    public TileBase floorTile;   // your plain blue/background tile
    public TileBase wallTile;

    [Header("Snake Tiles (Tile assets)")]
    public TileBase snakeHeadTile;
    public TileBase snakeBodyStraightTile;
    public TileBase snakeBodyTurnTile;
    public TileBase snakeTailTile;

    public Vector2Int StartPos { get; private set; }

    private bool[,] blocked;
    private bool[,] visited;

    private int openTileCount;
    private int visitedCount;

    // Track the exact order the player visited cells (the snake body)
    private readonly List<Vector3Int> path = new List<Vector3Int>();

    public bool IsComplete => visitedCount >= openTileCount;

    private enum Dir { Up, Right, Down, Left }

    private void Start()
    {
        // If player selected a level from the menu, use it.
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.SelectedLevel != null)
                level = GameManager.Instance.SelectedLevel;
            else if (GameManager.Instance.levelOrder.Count > 0)
            {
                GameManager.Instance.SelectedLevel = GameManager.Instance.levelOrder[0];
                level = GameManager.Instance.SelectedLevel;
            }
        }

        BuildLevel();
    }

    public void BuildLevel()
    {
        if (level == null) { Debug.LogError("No LevelData assigned."); return; }

        floorTilemap.ClearAllTiles();
        wallsTilemap.ClearAllTiles();
        if (snakeTilemap != null) snakeTilemap.ClearAllTiles();

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
                    floorTilemap.SetTile(cell, floorTile);
                    openTileCount++;

                    if (c == 'S')
                        StartPos = new Vector2Int(x, y);
                }
            }
        }

        // Initialize snake/path at start
        path.Clear();

        // Count start tile as visited (and draw snake)
        Visit(StartPos);

        // Move player to start position
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerObj.transform.position = CellToWorldCenter(StartPos);
        }
        else
        {
            Debug.LogWarning("Player with tag 'Player' not found. Make sure your Player GameObject has tag Player.");
        }
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

        // IMPORTANT CHANGE:
        // Do NOT paint red/visited tiles onto the floor anymore.
        // Floor stays as the normal floorTile.
        // Snake overlay draws on snakeTilemap instead.

        Vector3Int cell = new Vector3Int(p.x, p.y, 0);
        path.Add(cell);

        RedrawSnake();
    }

    public void Unvisit(Vector2Int p)
    {
        if (!InBounds(p) || blocked[p.x, p.y] || !visited[p.x, p.y]) return;

        visited[p.x, p.y] = false;
        visitedCount--;

        // Restore floor tile (optional, but keeps things consistent)
        floorTilemap.SetTile(new Vector3Int(p.x, p.y, 0), floorTile);

        // Remove from the path if present (handles undo/backtracking)
        Vector3Int cell = new Vector3Int(p.x, p.y, 0);
        int idx = path.LastIndexOf(cell);
        if (idx >= 0)
        {
            path.RemoveAt(idx);
        }

        RedrawSnake();
    }

    public Vector3 CellToWorldCenter(Vector2Int p)
    {
        return floorTilemap.GetCellCenterWorld(new Vector3Int(p.x, p.y, 0));
    }

    // -------------------------
    // Snake drawing logic
    // -------------------------

    private void RedrawSnake()
    {
        if (snakeTilemap == null) return;

        snakeTilemap.ClearAllTiles();

        if (path.Count == 0) return;

        // If only one cell visited: draw head only
        if (path.Count == 1)
        {
            Place(path[0], snakeHeadTile, 0);
            return;
        }

        // Tail (points toward the next segment)
        {
            var tailPos = path[0];
            var nextPos = path[1];
            var dir = DirFromTo(tailPos, nextPos);
            Place(tailPos, snakeTailTile, RotationFor(dir));
        }

        // Body (middle segments)
        for (int i = 1; i < path.Count - 1; i++)
        {
            var prev = path[i - 1];
            var cur = path[i];
            var next = path[i + 1];

            var inDir = DirFromTo(cur, prev);
            var outDir = DirFromTo(cur, next);

            if (IsStraight(inDir, outDir))
            {
                // If sprite is horizontal at 0°, rotate 90° for vertical
                int rot = (inDir == Dir.Left || inDir == Dir.Right) ? 0 : 90;
                Place(cur, snakeBodyStraightTile, rot);
            }
            else
            {
                int rot = RotationForTurn(inDir, outDir);
                Place(cur, snakeBodyTurnTile, rot);
            }
        }

        // Head (faces from previous segment toward head)
        {
            var headPos = path[path.Count - 1];
            var prevPos = path[path.Count - 2];
            var dir = DirFromTo(prevPos, headPos);
            Place(headPos, snakeHeadTile, RotationFor(dir));
        }
    }

    private Dir DirFromTo(Vector3Int a, Vector3Int b)
    {
        Vector3Int d = b - a;
        if (d == Vector3Int.up) return Dir.Up;
        if (d == Vector3Int.right) return Dir.Right;
        if (d == Vector3Int.down) return Dir.Down;
        return Dir.Left;
    }

    private bool IsStraight(Dir a, Dir b)
    {
        return (a == Dir.Left && b == Dir.Right) || (a == Dir.Right && b == Dir.Left) ||
               (a == Dir.Up && b == Dir.Down) || (a == Dir.Down && b == Dir.Up);
    }

    // Assumes your head/tail/straight sprites face RIGHT at 0°.
    private int RotationFor(Dir d)
    {
        return d switch
        {
            Dir.Right => 0,
            Dir.Up => 90,
            Dir.Left => 180,
            Dir.Down => 270,
            _ => 0
        };
    }

    // Assumes your turn sprite is a corner that connects Right+Up at 0°.
    // If your corner art is different, tell me and I’ll adjust this mapping.
    private int RotationForTurn(Dir inDir, Dir outDir)
    {
        if ((inDir == Dir.Right && outDir == Dir.Up) || (inDir == Dir.Up && outDir == Dir.Right)) return 0;
        if ((inDir == Dir.Up && outDir == Dir.Left) || (inDir == Dir.Left && outDir == Dir.Up)) return 90;
        if ((inDir == Dir.Left && outDir == Dir.Down) || (inDir == Dir.Down && outDir == Dir.Left)) return 180;
        return 270; // Down + Right
    }

    private void Place(Vector3Int cell, TileBase tile, int rotationDegrees)
    {
        if (tile == null) return;

        snakeTilemap.SetTile(cell, tile);

        Matrix4x4 m = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.Euler(0, 0, rotationDegrees),
            Vector3.one
        );

        snakeTilemap.SetTransformMatrix(cell, m);
    }
}
