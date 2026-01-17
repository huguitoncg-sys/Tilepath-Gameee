using System.Collections.Generic;
using UnityEngine;

public class PlayerGridMover : MonoBehaviour
{
    [Header("References")]
    public BoardManager board;
    public WinUIController winUI;

    [Header("Controls")]
    [Tooltip("Press this to undo one step (like Q in your Java file).")]
    public KeyCode undoKey = KeyCode.Q;

    [Tooltip("Press this to restart the level.")]
    public KeyCode restartKey = KeyCode.R;

    private Vector2Int pos;
    private readonly Stack<Vector2Int> history = new Stack<Vector2Int>();
    private bool hasWon;

    private void Start()
    {
        if (board == null) board = FindObjectOfType<BoardManager>();
        if (winUI == null) winUI = FindObjectOfType<WinUIController>();

        if (winUI != null) winUI.Hide();

        ResetToStart();
    }

    private void Update()
    {
        if (board == null) return;

        // Restart works anytime (even after winning)
        if (Input.GetKeyDown(restartKey))
        {
            RestartLevel();
            return;
        }

        // Allow undo even after winning (undo will hide the win UI)
        if (Input.GetKeyDown(undoKey))
        {
            Undo();
            return;
        }

        // If you won, freeze movement until restart (or undo)
        if (hasWon)
            return;

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
            hasWon = true;
            Debug.Log("WIN: all open tiles covered!");
            if (winUI != null) winUI.ShowWin();
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

        // If we had a win screen up, hide it once the board isn't complete anymore
        if (hasWon && !board.IsComplete)
        {
            hasWon = false;
            if (winUI != null) winUI.Hide();
        }
    }

    private void RestartLevel()
    {
        hasWon = false;
        if (winUI != null) winUI.Hide();

        board.BuildLevel();
        ResetToStart();
    }

    private void ResetToStart()
    {
        pos = board.StartPos;
        transform.position = board.CellToWorldCenter(pos);

        history.Clear();
        history.Push(pos);
    }
}
