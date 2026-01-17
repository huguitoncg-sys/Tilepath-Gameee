using UnityEngine;
using TMPro;

public class WinUIController : MonoBehaviour
{
    [Header("Assign these in the Inspector")]
    [Tooltip("The root GameObject of your win UI (Panel, Canvas child, etc.).")]
    public GameObject winPanel;

    [Tooltip("Optional: a TMP text field to show a message like 'You Win!'.")]
    public TMP_Text messageText;

    [TextArea]
    public string winMessage = "You Win!";

    private void Awake()
    {
        Hide();
    }

    public void ShowWin()
    {
        if (messageText != null)
            messageText.text = winMessage;

        if (winPanel != null)
            winPanel.SetActive(true);
    }

    public void Hide()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
    }
}