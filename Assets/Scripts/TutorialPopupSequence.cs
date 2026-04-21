using UnityEngine;
using UnityEngine.UI;

public class TutorialPopupSequence : MonoBehaviour
{
    [Header("Popup Canvases")]
    [SerializeField] private GameObject howToPlayCanvas;
    [SerializeField] private GameObject controlsCanvas;

    [Header("Buttons")]
    [SerializeField] private Button howToPlayGotItButton;
    [SerializeField] private Button controlsGotItButton;

    [Header("Settings")]
    [SerializeField] private bool pauseGameWhileOpen = true;

    private static bool hasShownTutorialThisSession = false;

    private float previousTimeScale = 1f;

    private bool previousCursorVisible;
    private CursorLockMode previousCursorLockState;

    private void Awake()
    {
        if (howToPlayCanvas != null)
            howToPlayCanvas.SetActive(false);

        if (controlsCanvas != null)
            controlsCanvas.SetActive(false);
    }

    private void Start()
    {
        if (hasShownTutorialThisSession)
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

    private void ShowHowToPlayPopup()
    {
        if (pauseGameWhileOpen)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        UnlockMouse();

        if (howToPlayCanvas != null)
            howToPlayCanvas.SetActive(true);

        if (controlsCanvas != null)
            controlsCanvas.SetActive(false);
    }

    private void ShowControlsPopup()
    {
        UnlockMouse();

        if (howToPlayCanvas != null)
            howToPlayCanvas.SetActive(false);

        if (controlsCanvas != null)
            controlsCanvas.SetActive(true);
    }

    private void FinishTutorial()
    {
        hasShownTutorialThisSession = true;

        HideAllPopups();

        if (pauseGameWhileOpen)
            Time.timeScale = previousTimeScale <= 0f ? 1f : previousTimeScale;

        RestoreMouse();
    }

    private void UnlockMouse()
    {
        previousCursorVisible = Cursor.visible;
        previousCursorLockState = Cursor.lockState;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void RestoreMouse()
    {
        Cursor.visible = previousCursorVisible;
        Cursor.lockState = previousCursorLockState;
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