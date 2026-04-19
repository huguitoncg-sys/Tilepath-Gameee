using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinUIController : MonoBehaviour
{
    [Header("Assign these in the Inspector")]
    public GameObject winPanel;
    public TMP_Text messageText;
    [TextArea] public string winMessage = "You Win!";

    [Header("Scene Loading")]
    public string mainMenuSceneName = "MainMenu";

    private bool isShowing;
    public bool IsShowing => isShowing;

    private void Awake()
    {
        Hide();
    }

    public void ShowWin()
    {
        if (isShowing) return;

        // Save completion for the currently selected level.
        if (GameManager.Instance != null && GameManager.Instance.SelectedLevel != null)
        {
            GameManager.Instance.MarkLevelCompleted(GameManager.Instance.SelectedLevel);
        }

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
        Hide();

        if (GameManager.Instance == null)
        {
            Debug.LogError("No GameManager found.");
            return;
        }

        if (!GameManager.Instance.TryAdvanceToNextLevel())
        {
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}