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

    [Header("Level Order")]
    public List<LevelData> levelOrder = new List<LevelData>();

    [Header("Score")]
    [SerializeField] private int score = 0;
    public int Score => score;

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

        ClearAllCompletionData();
        CheckForDuplicateLevels();
    }

    private void Start()
    {
        if (SelectedLevel != null && !levelOrder.Contains(SelectedLevel))
        {
            Debug.LogWarning("SelectedLevel is not inside GameManager levelOrder: " + SelectedLevel.name);
        }
    }

    // ---------- LEVEL HELPERS ----------

    public void SelectLevel(LevelData level)
    {
        if (level == null)
        {
            Debug.LogWarning("Tried to select a null level.");
            return;
        }

        SelectedLevel = level;

        int index = GetSelectedLevelIndex();

        if (index < 0)
        {
            Debug.LogWarning("Selected level is not in levelOrder: " + level.name);
        }
        else
        {
            Debug.Log("Selected level: " + level.name + " | Global level number: " + (index + 1));
        }

        OnLevelChanged?.Invoke();
    }

    public int GetSelectedLevelIndex()
    {
        if (SelectedLevel == null)
        {
            return -1;
        }

        return levelOrder.IndexOf(SelectedLevel);
    }

    public int GetSelectedLevelNumber()
    {
        int index = GetSelectedLevelIndex();

        if (index >= 0)
        {
            return index + 1;
        }

        return 0;
    }

    public bool TryAdvanceToNextLevel()
    {
        int currentIndex = GetSelectedLevelIndex();

        if (currentIndex < 0)
        {
            Debug.LogError("Cannot advance. Current SelectedLevel is not in GameManager.levelOrder.");
            return false;
        }

        int nextIndex = currentIndex + 1;

        if (nextIndex >= levelOrder.Count)
        {
            Debug.Log("No more levels after this one.");
            return false;
        }

        SelectedLevel = levelOrder[nextIndex];

        Debug.Log("Advanced to level: " + SelectedLevel.name + " | Global level number: " + (nextIndex + 1));

        OnLevelChanged?.Invoke();
        return true;
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
        {
            return string.Empty;
        }

        return level.name;
    }

    public void MarkLevelCompleted(LevelData level)
    {
        if (level == null)
        {
            return;
        }

        string key = GetLevelCompletionKey(level);
        completedLevels.Add(key);
    }

    public bool IsLevelCompleted(LevelData level)
    {
        if (level == null)
        {
            return false;
        }

        string key = GetLevelCompletionKey(level);
        return completedLevels.Contains(key);
    }

    public bool IsWorldCompleted(WorldData world)
    {
        if (world == null || world.levels == null || world.levels.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < world.levels.Length; i++)
        {
            if (!IsLevelCompleted(world.levels[i]))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearAllCompletionData()
    {
        completedLevels.Clear();
    }

    // ---------- DEBUG HELPERS ----------

    private void CheckForDuplicateLevels()
    {
        HashSet<LevelData> seenLevels = new HashSet<LevelData>();

        for (int i = 0; i < levelOrder.Count; i++)
        {
            LevelData level = levelOrder[i];

            if (level == null)
            {
                Debug.LogWarning("GameManager levelOrder has an empty slot at index " + i);
                continue;
            }

            if (seenLevels.Contains(level))
            {
                Debug.LogWarning("Duplicate LevelData found in levelOrder: " + level.name);
            }
            else
            {
                seenLevels.Add(level);
            }
        }
    }
}