using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Fired whenever SelectedLevel changes (selecting from menu, or advancing).
    public event Action OnLevelChanged;

    // Fired whenever score changes (HUD can listen to this if you want).
    public event Action<int> OnScoreChanged;

    [Header("Selected Level")]
    public LevelData SelectedLevel;

    [Header("Level Order (drag your LevelData assets here in order)")]
    public List<LevelData> levelOrder = new List<LevelData>();

    [Header("Score")]
    [SerializeField] private int score = 0;

    public int Score => score;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ---------- LEVEL HELPERS ----------

    public int GetSelectedLevelIndex()
    {
        if (SelectedLevel == null) return -1;
        return levelOrder.IndexOf(SelectedLevel);
    }

    // 1-based number for UI: Level 1, Level 2, etc.
    public int GetSelectedLevelNumber()
    {
        int idx = GetSelectedLevelIndex();   // 0-based
        return (idx >= 0) ? idx + 1 : 0;     // 1-based for UI
    }

    public bool TryAdvanceToNextLevel()
    {
        int i = GetSelectedLevelIndex();
        if (i < 0) return false;

        int next = i + 1;
        if (next >= levelOrder.Count) return false;

        SelectedLevel = levelOrder[next];
        OnLevelChanged?.Invoke();
        return true;
    }

    public void SelectLevel(LevelData level)
    {
        SelectedLevel = level;

        // Let HUD/Board refresh immediately.
        OnLevelChanged?.Invoke();

        // Optional: warn if it isn't in your order list
        if (level != null && !levelOrder.Contains(level))
            Debug.LogWarning($"SelectedLevel '{level.name}' is not in GameManager.levelOrder list.");
    }

    // ---------- SCORE ----------

    public void ResetScore()
    {
        score = 0;
        Debug.Log("[GameManager] Score reset to 0");
        OnScoreChanged?.Invoke(score);
    }

    public void AddScore(int amount = 1)
    {
        score += amount;
        Debug.Log($"[GameManager] Score +{amount} => {score}");
        OnScoreChanged?.Invoke(score);
    }

    // Optional helper if you want to set it directly.
    public void SetScore(int value)
    {
        score = Mathf.Max(0, value);
        Debug.Log($"[GameManager] Score set => {score}");
        OnScoreChanged?.Invoke(score);
    }
}