using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SnakeTrailRenderer : MonoBehaviour
{
    [Header("Where to draw the snake")]
    public Tilemap snakeTilemap;

    [Header("Snake tiles (Tile assets)")]
    public TileBase headTile;
    public TileBase bodyStraightTile;
    public TileBase bodyTurnTile;
    public TileBase tailTile;

    [Header("References")]
    public BoardManager boardManager;

    private readonly List<Vector3Int> path = new List<Vector3Int>();

    public event Action OnLevelComplete;

    private bool hasCompleted = false;

    private enum Dir { Up, Right, Down, Left }

    private void Awake()
    {
        if (boardManager == null)
        {
            boardManager = GetComponent<BoardManager>();

            if (boardManager == null)
            {
                boardManager = FindObjectOfType<BoardManager>();
            }
        }
    }

    public void ResetSnake(Vector3Int startCell)
    {
        if (snakeTilemap != null)
        {
            snakeTilemap.ClearAllTiles();
        }

        path.Clear();
        path.Add(startCell);
        hasCompleted = false;
        Redraw();
    }

    public void AddStep(Vector3Int newCell)
    {
        path.Add(newCell);
        Redraw();
    }

    private void Redraw()
    {
        if (snakeTilemap == null) return;

        snakeTilemap.ClearAllTiles();

        if (path.Count == 0) return;

        if (path.Count == 1)
        {
            Vector2Int startDir = Vector2Int.right;

            if (boardManager != null)
            {
                startDir = boardManager.GetStartFacingDir();
            }

            Place(path[0], headTile, RotationFor(startDir));
            return;
        }

        // Tail
        {
            Vector3Int tailPos = path[0];
            Vector3Int nextPos = path[1];
            Dir dir = DirFromTo(tailPos, nextPos);
            Place(tailPos, tailTile, RotationFor(dir));
        }

        // Body
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
                Place(cur, bodyStraightTile, rot);
            }
            else
            {
                int rot = RotationForTurn(inDir, outDir);
                Place(cur, bodyTurnTile, rot);
            }
        }

        // Head
        {
            Vector3Int headPos = path[path.Count - 1];
            Vector3Int prevPos = path[path.Count - 2];
            Dir dir = DirFromTo(prevPos, headPos);
            Place(headPos, headTile, RotationFor(dir));
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