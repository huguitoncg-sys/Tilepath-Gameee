using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinUIController : MonoBehaviour
{
    [Header("Assign these in the Inspector")]
    [Tooltip("The root GameObject of your win UI (Panel, Canvas child, etc.).")]
    public GameObject winPanel;

    [Tooltip("Optional: a TMP text field to show a message like 'You Win!'")]
    public TMP_Text messageText;

    [TextArea]
    public string winMessage = "You Win!";

    [Header("Scene Loading")]
    [Tooltip("Name of your Main Menu scene in Build Settings")]
    public string mainMenuSceneName = "MainMenu";

    private bool isShowing;

    private void Awake()
    {
        Hide(); // starts hidden and ensures timeScale is normal
    }

    // Call this when the player wins
    public void ShowWin()
    {
        if (isShowing) return;
        isShowing = true;

        if (messageText != null)
            messageText.text = winMessage;

        if (winPanel != null)
            winPanel.SetActive(true);

        // Actually pause the game
        Time.timeScale = 0f;

        // Let the player click UI
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Hide()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        isShowing = false;

        // Ensure game is running if panel is hidden
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // ---- Button Methods ----

    // OnClick for "Main Menu"
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // OnClick for "Next Level"
    public void NextLevel()
    {
        Time.timeScale = 1f;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            // No more levels -> back to menu (or you can show a "Game Complete" screen)
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        SceneManager.LoadScene(nextIndex);
    }
}
