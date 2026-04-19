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
        RefreshWorldButtons();
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
            string displayText = (i + 1).ToString("D2");

            btn.Setup(level, this, displayText);
        }
    }

    private void ClearLevelButtons()
    {
        for (int i = levelGridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(levelGridParent.GetChild(i).gameObject);
        }
    }

    private void RefreshWorldButtons()
    {
        WorldSelectButton[] worldButtons = worldPanel.GetComponentsInChildren<WorldSelectButton>(true);

        for (int i = 0; i < worldButtons.Length; i++)
        {
            worldButtons[i].RefreshVisuals();
        }
    }

    public void OnLevelChosen(LevelData level)
    {
        GameManager.Instance.SelectLevel(level);
        SceneManager.LoadScene(gameSceneName);
    }

    public void ChooseLevel(LevelData level)
    {
        GameManager.Instance.SelectLevel(level);
        SceneManager.LoadScene(gameSceneName);
    }
}