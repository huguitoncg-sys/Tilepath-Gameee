using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Selected Level")]
    public LevelData SelectedLevel;

    [Header("Level Order (drag your LevelData assets here in order)")]
    public List<LevelData> levelOrder = new List<LevelData>();

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

    public int GetSelectedLevelIndex()
    {
        if (SelectedLevel == null) return -1;
        return levelOrder.IndexOf(SelectedLevel);
    }

    public bool TryAdvanceToNextLevel()
    {
        int i = GetSelectedLevelIndex();
        if (i < 0) return false;

        int next = i + 1;
        if (next >= levelOrder.Count) return false;

        SelectedLevel = levelOrder[next];
        return true;
    }
    public void SelectLevel(LevelData level)
    {
        SelectedLevel = level;
    }
    

}
