using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinUIController : MonoBehaviour
{
    [Header("Assign these in the Inspector")]
    public GameObject winPanel;
    public TMP_Text messageText;

    [TextArea]
    public string winMessage = "You Win!";

    [Header("Scene Loading")]
    public string mainMenuSceneName = "MainMenu";

    private bool isShowing;

    private void Awake()
    {
        Hide();
    }

    public void ShowWin()
    {
        if (isShowing) return;
        isShowing = true;

        if (messageText != null)
            messageText.text = winMessage;

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Hide()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        isShowing = false;

        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void NextLevel()
{
    // ✅ Use Hide() so isShowing resets properly
    Hide();

    if (GameManager.Instance == null)
    {
        Debug.LogError("No GameManager found.");
        return;
    }

    // Advance to next ScriptableObject level
    if (!GameManager.Instance.TryAdvanceToNextLevel())
    {
        SceneManager.LoadScene(mainMenuSceneName);
        return;
    }

    // Rebuild the SAME gameplay scene with the new LevelData
    BoardManager bm = FindObjectOfType<BoardManager>();
    if (bm == null)
    {
        Debug.LogError("No BoardManager found.");
        return;
    }

    bm.level = GameManager.Instance.SelectedLevel;
    bm.BuildLevel();

    // ✅ Reset the player’s internal state so movement works again
    PlayerGridMover player = FindObjectOfType<PlayerGridMover>();
    if (player != null)
    {
        player.ResetForNewLevel(bm);
    }
    else
    {
        Debug.LogWarning("No PlayerGridMover found in scene.");
    }
}
}
