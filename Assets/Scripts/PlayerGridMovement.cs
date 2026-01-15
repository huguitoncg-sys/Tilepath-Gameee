using System.Collections.Generic;
using UnityEngine;

public class PlayerGridMover : MonoBehaviour
{
    public BoardManager board;

    [Tooltip("Press this to undo one step (like Q in your Java file).")]
    public KeyCode undoKey = KeyCode.Q;

    private Vector2Int pos;
    private Stack<Vector2Int> history = new Stack<Vector2Int>();

    private void Start()
    {
        if (board == null) board = FindObjectOfType<BoardManager>();

        pos = board.StartPos;
        transform.position = board.CellToWorldCenter(pos);
        history.Push(pos); // start in history
    }

    private void Update()
    {
        if (board == null) return;

        if (Input.GetKeyDown(undoKey))
        {
            Undo();
            return;
        }

        Vector2Int dir = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) dir = Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) dir = Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) dir = Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) dir = Vector2Int.right;

        if (dir != Vector2Int.zero)
            TryMove(dir);
    }

    private void TryMove(Vector2Int dir)
    {
        Vector2Int next = pos + dir;

        if (!board.CanStep(next))
            return;

        pos = next;
        history.Push(pos);

        board.Visit(pos);
        transform.position = board.CellToWorldCenter(pos);

        if (board.IsComplete)
        {
            Debug.Log("WIN: all open tiles covered!");
            // Hook up UI, next level, etc.
        }
    }

    private void Undo()
    {
        if (history.Count <= 1) return; // keep start

        // current position gets unvisited
        Vector2Int current = history.Pop();
        board.Unvisit(current);

        pos = history.Peek();
        transform.position = board.CellToWorldCenter(pos);
    }
}
