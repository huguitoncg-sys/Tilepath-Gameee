using UnityEngine;
using UnityEngine.UI;

public class TutorialPopupSequence : MonoBehaviour
{
    public static bool IsTutorialOpen { get; private set; }
    private static bool hasShownThisSession = false;

    [Header("Popup Canvases")]
    [SerializeField] private GameObject howToPlayCanvas;
    [SerializeField] private GameObject controlsCanvas;

    [Header("Buttons")]
    [SerializeField] private Button howToPlayGotItButton;
    [SerializeField] private Button controlsGotItButton;

    private void Awake()
    {
        IsTutorialOpen = false;

        if (howToPlayCanvas != null)
            howToPlayCanvas.SetActive(false);

        if (controlsCanvas != null)
            controlsCanvas.SetActive(false);
    }

    private void Start()
    {
        if (hasShownThisSession)
        {
            HideAllPopups();
            return;
        }

        if (howToPlayGotItButton != null)
            howToPlayGotItButton.onClick.AddListener(ShowControlsPopup);

        if (controlsGotItButton != null)
            controlsGotItButton.onClick.AddListener(FinishTutorial);

        ShowHowToPlayPopup();
    }

    private void LateUpdate()
    {
        // This is the important fix.
        // LateUpdate runs after most Update logic, so it overrides anything else
        // that tries to hide or lock the cursor.
        if (IsTutorialOpen)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void ShowHowToPlayPopup()
    {
        IsTutorialOpen = true;

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (howToPlayCanvas != null)
            howToPlayCanvas.SetActive(true);

        if (controlsCanvas != null)
            controlsCanvas.SetActive(false);
    }

    private void ShowControlsPopup()
    {
        IsTutorialOpen = true;

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (howToPlayCanvas != null)
            howToPlayCanvas.SetActive(false);

        if (controlsCanvas != null)
            controlsCanvas.SetActive(true);
    }

    private void FinishTutorial()
    {
        hasShownThisSession = true;
        IsTutorialOpen = false;

        HideAllPopups();

        Time.timeScale = 1f;

        // Keep the mouse visible after the tutorial.
        // This is better for your game because you use UI buttons often.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideAllPopups()
    {
        if (howToPlayCanvas != null)
            howToPlayCanvas.SetActive(false);

        if (controlsCanvas != null)
            controlsCanvas.SetActive(false);
    }

    private void OnDestroy()
    {
        if (howToPlayGotItButton != null)
            howToPlayGotItButton.onClick.RemoveListener(ShowControlsPopup);

        if (controlsGotItButton != null)
            controlsGotItButton.onClick.RemoveListener(FinishTutorial);
    }
}