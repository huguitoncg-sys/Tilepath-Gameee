using System.Collections.Generic;
using UnityEngine;

public class PlayerGridMover : MonoBehaviour
{
    [Header("References")]
    public BoardManager board;
    public WinUIController winUI;

    [Header("Controls")]
    [Tooltip("Press this to undo one step.")]
    public KeyCode undoKey = KeyCode.Q;

    [Tooltip("Press this to restart the level.")]
    public KeyCode restartKey = KeyCode.R;

    [Header("Audio")]
    [Tooltip("AudioSource used to play the movement sound.")]
    public AudioSource audioSource;

    [Tooltip("Sound effect that plays whenever the snake moves successfully.")]
    public AudioClip moveClip;

    [Range(0f, 1f)]
    public float moveVolume = 1f;

    private Vector2Int pos;
    private readonly Stack<Vector2Int> history = new Stack<Vector2Int>();
    private bool hasWon;

    private void Start()
    {
        if (board == null) board = FindObjectOfType<BoardManager>();
        if (winUI == null) winUI = FindObjectOfType<WinUIController>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // Make sure the AudioSource is set up for sound effects
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        if (winUI != null) winUI.Hide();

        ResetToStart();
    }

    private void Update()
{
    if (board == null) return;

    if (TutorialPopupSequence.IsTutorialOpen) return;

    // Restart works anytime
    if (Input.GetKeyDown(restartKey))
    {
        RestartLevel();
        return;
    }

    // Undo works anytime
    if (Input.GetKeyDown(undoKey))
    {
        Undo();
        return;
    }

    // Stop movement after win
    if (hasWon) return;

    Vector2Int dir = Vector2Int.zero;

    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        dir = Vector2Int.up;
    else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        dir = Vector2Int.down;
    else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        dir = Vector2Int.left;
    else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        dir = Vector2Int.right;

    if (dir != Vector2Int.zero)
        TryMove(dir);
}

    private void TryMove(Vector2Int dir)
    {
        Vector2Int next = pos + dir;

        // Do nothing if move is invalid
        if (!board.CanStep(next))
            return;

        // Update player position
        pos = next;
        history.Push(pos);

        board.Visit(pos);
        transform.position = board.CellToWorldCenter(pos);

        // Play move sound after successful move
        PlayMoveSound();

        // Check win
        if (board.IsComplete)
        {
            hasWon = true;
            Debug.Log("WIN: all open tiles covered!");
            if (winUI != null) winUI.ShowWin();
        }
    }

    private void PlayMoveSound()
    {
        if (audioSource != null && moveClip != null)
        {
            audioSource.PlayOneShot(moveClip, moveVolume);
        }
    }

    private void Undo()
    {
        if (history.Count <= 1) return; // keep starting position

        Vector2Int current = history.Pop();
        board.Unvisit(current);

        pos = history.Peek();
        transform.position = board.CellToWorldCenter(pos);

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

    public void ResetForNewLevel(BoardManager newBoard)
    {
        board = newBoard;

        hasWon = false;
        if (winUI != null) winUI.Hide();

        pos = board.StartPos;
        transform.position = board.CellToWorldCenter(pos);

        history.Clear();
        history.Push(pos);

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = true;
            rb.WakeUp();
        }
    }
}