using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action OnLevelChanged;
    public event Action<int> OnScoreChanged;

    [Header("Selected Level")]
    public LevelData SelectedLevel;

    [Header("Level Order (drag your LevelData assets here in order)")]
    public List<LevelData> levelOrder = new List<LevelData>();

    [Header("Score")]
    [SerializeField] private int score = 0;
    public int Score => score;

    // This only saves progress while the game is running.
    // When you stop Play Mode or quit the build, it resets.
    private HashSet<string> completedLevels = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Makes sure every new run starts clean.
        ClearAllCompletionData();
    }

    // ---------- LEVEL HELPERS ----------

    public int GetSelectedLevelIndex()
    {
        if (SelectedLevel == null)
            return -1;

        return levelOrder.IndexOf(SelectedLevel);
    }

    public int GetSelectedLevelNumber()
    {
        int index = GetSelectedLevelIndex();

        if (index >= 0)
            return index + 1;

        return 0;
    }

    public bool TryAdvanceToNextLevel()
    {
        int currentIndex = GetSelectedLevelIndex();

        if (currentIndex < 0)
            return false;

        int nextIndex = currentIndex + 1;

        if (nextIndex >= levelOrder.Count)
            return false;

        SelectedLevel = levelOrder[nextIndex];
        OnLevelChanged?.Invoke();

        return true;
    }

    public void SelectLevel(LevelData level)
    {
        SelectedLevel = level;
        OnLevelChanged?.Invoke();

        if (level != null && !levelOrder.Contains(level))
        {
            Debug.LogWarning($"SelectedLevel '{level.name}' is not in GameManager.levelOrder list.");
        }
    }

    // ---------- SCORE HELPERS ----------

    public void ResetScore()
    {
        score = 0;
        OnScoreChanged?.Invoke(score);
    }

    public void AddScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);
    }

    // ---------- COMPLETION HELPERS ----------

    private string GetLevelCompletionKey(LevelData level)
    {
        if (level == null)
            return string.Empty;

        return level.name;
    }

    public void MarkLevelCompleted(LevelData level)
    {
        if (level == null)
            return;

        string key = GetLevelCompletionKey(level);
        completedLevels.Add(key);
    }

    public bool IsLevelCompleted(LevelData level)
    {
        if (level == null)
            return false;

        string key = GetLevelCompletionKey(level);
        return completedLevels.Contains(key);
    }

    public bool IsWorldCompleted(WorldData world)
    {
        if (world == null || world.levels == null || world.levels.Length == 0)
            return false;

        for (int i = 0; i < world.levels.Length; i++)
        {
            if (!IsLevelCompleted(world.levels[i]))
                return false;
        }

        return true;
    }

    public void ClearAllCompletionData()
    {
        completedLevels.Clear();
    }
}