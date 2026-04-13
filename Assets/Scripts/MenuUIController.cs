using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject worldPanel;
    public GameObject levelPanel;

    [Header("Level List UI")]
    public Transform levelGridParent;
    public GameObject levelButtonPrefab;

    [Header("Worlds")]
    public WorldData[] worlds;

    [Header("Scene Names")]
    public string gameSceneName = "Game";

    private void Start()
    {
        ShowWorlds();
    }

    public void ShowWorlds()
    {
        worldPanel.SetActive(true);
        levelPanel.SetActive(false);
        ClearLevelButtons();
    }

    public void ShowLevels(WorldData world)
    {
        worldPanel.SetActive(false);
        levelPanel.SetActive(true);

        ClearLevelButtons();

        for (int i = 0; i < world.levels.Length; i++)
        {
            LevelData level = world.levels[i];

            GameObject btnObj = Instantiate(levelButtonPrefab, levelGridParent);
            LevelSelectButton btn = btnObj.GetComponent<LevelSelectButton>();
            btn.Setup(level, this);
        }
    }

    private void ClearLevelButtons()
    {
        for (int i = levelGridParent.childCount - 1; i >= 0; i--)
            Destroy(levelGridParent.GetChild(i).gameObject);
    }

    public void OnLevelChosen(LevelData level)
    {
        GameManager.Instance.SelectLevel(level);
        SceneManager.LoadScene(gameSceneName);
    }

    public void ChooseLevel(LevelData level)
    {
        GameManager.Instance.SelectLevel(level);
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }
}