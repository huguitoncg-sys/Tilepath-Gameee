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
    public Tilemap snakeTilemap;

    [Header("Board Tiles")]
    public TileBase floorTile;
    public TileBase wallTile;

    [Header("Snake Tiles (Tile assets)")]
    public TileBase snakeHeadTile;
    public TileBase snakeBodyStraightTile;
    public TileBase snakeBodyTurnTile;
    public TileBase snakeTailTile;

    [Header("Collectibles")]
    public GameObject applePrefab;

    private GameObject currentApple;
    private Vector2Int fruitCell = new Vector2Int(-1, -1);

    public Vector2Int StartPos { get; private set; }

    private bool[,] blocked;
    private bool[,] visited;

    private int openTileCount;
    private int visitedCount;

    private readonly List<Vector3Int> path = new List<Vector3Int>();

    public bool IsComplete => visitedCount >= openTileCount;

    private enum Dir { Up, Right, Down, Left }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelChanged += HandleLevelChanged;

            if (GameManager.Instance.SelectedLevel == null && GameManager.Instance.levelOrder.Count > 0)
            {
                GameManager.Instance.SelectLevel(GameManager.Instance.levelOrder[0]);
            }

            if (GameManager.Instance.SelectedLevel != null)
            {
                HandleLevelChanged();
            }
        }
        else
        {
            BuildLevel();
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelChanged -= HandleLevelChanged;
        }
    }

    private void HandleLevelChanged()
    {
        level = GameManager.Instance.SelectedLevel;
        BuildLevel();
    }

    public void BuildLevel()
    {
        if (level == null)
        {
            Debug.LogError("No LevelData assigned.");
            return;
        }

        floorTilemap.ClearAllTiles();
        wallsTilemap.ClearAllTiles();

        if (snakeTilemap != null)
        {
            snakeTilemap.ClearAllTiles();
        }

        if (currentApple != null)
        {
            Destroy(currentApple);
            currentApple = null;
        }

        int w = level.Width;
        int h = level.Height;

        blocked = new bool[w, h];
        visited = new bool[w, h];

        openTileCount = 0;
        visitedCount = 0;
        StartPos = Vector2Int.zero;
        fruitCell = new Vector2Int(-1, -1);

        for (int row = 0; row < h; row++)
        {
            string line = level.rows[row];

            for (int x = 0; x < w; x++)
            {
                char c = line[x];
                int y = (h - 1) - row;

                Vector3Int cell = new Vector3Int(x, y, 0);

                if (c == '#')
                {
                    blocked[x, y] = true;
                    wallsTilemap.SetTile(cell, wallTile);
                }
                else
                {
                    floorTilemap.SetTile(cell, floorTile);
                    openTileCount++;

                    if (c == 'S')
                    {
                        StartPos = new Vector2Int(x, y);
                    }

                    if (c == 'F')
                    {
                        fruitCell = new Vector2Int(x, y);
                    }
                }
            }
        }

        path.Clear();
        Visit(StartPos);

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerObj.transform.position = CellToWorldCenter(StartPos);
        }
        else
        {
            Debug.LogWarning("Player with tag 'Player' not found. Make sure your Player GameObject has tag Player.");
        }

        SpawnFruitIfPresent();

            BoardCameraFitter cameraFitter = Camera.main != null
                ? Camera.main.GetComponent<BoardCameraFitter>()
                : null;

            if (cameraFitter != null)
            {
                cameraFitter.FitToBoard();
            }
    }

    private void SpawnFruitIfPresent()
    {
        if (applePrefab == null) return;
        if (fruitCell.x < 0) return;

        Vector3 spawnPos = CellToWorldCenter(fruitCell);
        currentApple = Instantiate(applePrefab, spawnPos, Quaternion.identity);
    }

    public bool InBounds(Vector2Int p)
    {
        return blocked != null &&
               p.x >= 0 &&
               p.y >= 0 &&
               p.x < blocked.GetLength(0) &&
               p.y < blocked.GetLength(1);
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

        Vector3Int cell = new Vector3Int(p.x, p.y, 0);
        path.Add(cell);

        RedrawSnake();
    }

    public void Unvisit(Vector2Int p)
    {
        if (!InBounds(p) || blocked[p.x, p.y] || !visited[p.x, p.y]) return;

        visited[p.x, p.y] = false;
        visitedCount--;

        floorTilemap.SetTile(new Vector3Int(p.x, p.y, 0), floorTile);

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

    public Vector2Int GetStartFacingDir()
    {
        List<Vector2Int> openDirs = GetOpenDirections(StartPos);

        if (openDirs.Count == 0)
            return Vector2Int.down;

        if (openDirs.Count == 1)
            return openDirs[0];

        int width = blocked.GetLength(0);
        int height = blocked.GetLength(1);

        bool onLeftEdge = StartPos.x == 0;
        bool onRightEdge = StartPos.x == width - 1;
        bool onBottomEdge = StartPos.y == 0;
        bool onTopEdge = StartPos.y == height - 1;

        if (onLeftEdge && openDirs.Contains(Vector2Int.right))
            return Vector2Int.right;

        if (onRightEdge && openDirs.Contains(Vector2Int.left))
            return Vector2Int.left;

        if (onTopEdge && openDirs.Contains(Vector2Int.down))
            return Vector2Int.down;

        if (onBottomEdge && openDirs.Contains(Vector2Int.up))
            return Vector2Int.up;

        Vector2Int[] interiorPreference =
        {
            Vector2Int.down,
            Vector2Int.right,
            Vector2Int.left,
            Vector2Int.up
        };

        foreach (Vector2Int dir in interiorPreference)
        {
            if (openDirs.Contains(dir))
                return dir;
        }

        return openDirs[0];
    }

    private List<Vector2Int> GetOpenDirections(Vector2Int origin)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int next = origin + dir;
            if (InBounds(next) && !blocked[next.x, next.y])
            {
                result.Add(dir);
            }
        }

        return result;
    }

    private void RedrawSnake()
    {
        if (snakeTilemap == null) return;

        snakeTilemap.ClearAllTiles();

        if (path.Count == 0) return;

        if (path.Count == 1)
        {
            Vector2Int startDir = GetStartFacingDir();
            Place(path[0], snakeHeadTile, RotationFor(startDir));
            return;
        }

        {
            Vector3Int tailPos = path[0];
            Vector3Int nextPos = path[1];
            Dir tailDir = DirFromTo(tailPos, nextPos);
            Place(tailPos, snakeTailTile, RotationFor(tailDir));
        }

        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector3Int prev = path[i - 1];
            Vector3Int cur = path[i];
            Vector3Int next = path[i + 1];

            Dir inDir = DirFromTo(cur, prev);
            Dir outDir = DirFromTo(cur, next);

            if (IsStraight(inDir, outDir))
            {
                int rot = (inDir == Dir.Left || inDir == Dir.Right) ? 0 : 90;
                Place(cur, snakeBodyStraightTile, rot);
            }
            else
            {
                int rot = RotationForTurn(inDir, outDir);
                Place(cur, snakeBodyTurnTile, rot);
            }
        }

        {
            Vector3Int headPos = path[path.Count - 1];
            Vector3Int prevPos = path[path.Count - 2];
            Dir headDir = DirFromTo(prevPos, headPos);
            Place(headPos, snakeHeadTile, RotationFor(headDir));
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
        return (a == Dir.Left && b == Dir.Right) ||
               (a == Dir.Right && b == Dir.Left) ||
               (a == Dir.Up && b == Dir.Down) ||
               (a == Dir.Down && b == Dir.Up);
    }

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

    private int RotationFor(Vector2Int dir)
    {
        if (dir == Vector2Int.right) return 0;
        if (dir == Vector2Int.up) return 90;
        if (dir == Vector2Int.left) return 180;
        if (dir == Vector2Int.down) return 270;
        return 0;
    }

    private int RotationForTurn(Dir inDir, Dir outDir)
    {
        if ((inDir == Dir.Right && outDir == Dir.Up) || (inDir == Dir.Up && outDir == Dir.Right)) return 0;
        if ((inDir == Dir.Up && outDir == Dir.Left) || (inDir == Dir.Left && outDir == Dir.Up)) return 90;
        if ((inDir == Dir.Left && outDir == Dir.Down) || (inDir == Dir.Down && outDir == Dir.Left)) return 180;
        return 270;
    }

    private void Place(Vector3Int cell, TileBase tile, int rotationDegrees)
    {
        if (tile == null || snakeTilemap == null) return;

        snakeTilemap.SetTile(cell, tile);

        Matrix4x4 m = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.Euler(0f, 0f, rotationDegrees),
            Vector3.one
        );

        snakeTilemap.SetTransformMatrix(cell, m);
    }
}