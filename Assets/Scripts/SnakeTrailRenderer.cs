using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;


public class SnakeTrailRenderer : MonoBehaviour
{
    [Header("Where to draw the snake")]
    public Tilemap snakeTilemap;

    [Header("Snake tiles (Tile assets)")]
    public TileBase headTile;
    public TileBase bodyStraightTile;
    public TileBase bodyTurnTile;
    public TileBase tailTile;

    // Ordered path of visited cells (snake body)
    private readonly List<Vector3Int> path = new List<Vector3Int>();

    // Call this once when level starts

    public event Action OnLevelComplete;

    private bool hasCompleted = false; 

    public void ResetSnake(Vector3Int startCell)
    {
        snakeTilemap.ClearAllTiles();
        path.Clear();
        path.Add(startCell);
        Redraw();
    }

    // Call this every time the player successfully moves to a new cell
    public void AddStep(Vector3Int newCell)
    {
        path.Add(newCell);
        Redraw();
    }

    private void Redraw()
    {
        snakeTilemap.ClearAllTiles();

        if (path.Count == 0) return;

        if (path.Count == 1)
        {
            Place(path[0], headTile, 0);
            return;
        }

        // Tail (segment 0 points toward segment 1)
        {
            var tailPos = path[0];
            var nextPos = path[1];
            var dir = DirFromTo(tailPos, nextPos);
            Place(tailPos, tailTile, RotationFor(dir));
        }

        // Body (segments 1..n-2)
        for (int i = 1; i < path.Count - 1; i++)
        {
            var prev = path[i - 1];
            var cur  = path[i];
            var next = path[i + 1];

            var inDir  = DirFromTo(cur, prev); // direction from cur toward prev
            var outDir = DirFromTo(cur, next); // direction from cur toward next

            if (IsStraight(inDir, outDir))
            {
                // Horizontal vs vertical decides rotation
                int rot = (inDir == Dir.Left || inDir == Dir.Right) ? 0 : 90;
                Place(cur, bodyStraightTile, rot);
            }
            else
            {
                // Turn tile: rotate based on the corner shape
                int rot = RotationForTurn(inDir, outDir);
                Place(cur, bodyTurnTile, rot);
            }
        }

        // Head (last segment faces from previous to head)
        {
            var headPos = path[path.Count - 1];
            var prevPos = path[path.Count - 2];
            var dir = DirFromTo(prevPos, headPos);
            Place(headPos, headTile, RotationFor(dir));
        }
    }

    private enum Dir { Up, Right, Down, Left }

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
               (a == Dir.Up && b == Dir.Down)   || (a == Dir.Down && b == Dir.Up);
    }

    // Rotation degrees for sprites that face "Right" at 0 degrees
    private int RotationFor(Dir d)
    {
        return d switch
        {
            Dir.Right => 0,
            Dir.Up    => 90,
            Dir.Left  => 180,
            Dir.Down  => 270,
            _ => 0
        };
    }

    // Assumes your turn sprite is a corner that connects Right+Up at 0 degrees.
    // If your corner sprite is different, we’ll tweak this mapping.
    private int RotationForTurn(Dir inDir, Dir outDir)
    {
        // Order doesn't matter for a corner, so normalize pairs
        if ((inDir == Dir.Right && outDir == Dir.Up) || (inDir == Dir.Up && outDir == Dir.Right)) return 0;
        if ((inDir == Dir.Up && outDir == Dir.Left) || (inDir == Dir.Left && outDir == Dir.Up)) return 90;
        if ((inDir == Dir.Left && outDir == Dir.Down) || (inDir == Dir.Down && outDir == Dir.Left)) return 180;
        // Down + Right
        return 270;
    }

    private void Place(Vector3Int cell, TileBase tile, int rotationDegrees)
    {
        snakeTilemap.SetTile(cell, tile);

        // Rotate the tile by setting its transform matrix
        Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, rotationDegrees), Vector3.one);
        snakeTilemap.SetTransformMatrix(cell, m);
    }

    
}
