using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text label;
    public Image targetImage;
    public Button button;

    [Header("Sprites")]
    public Sprite defaultSprite;
    public Sprite completedSprite;

    private LevelData level;
    private MenuUIController menu;

    public void Setup(LevelData level, MenuUIController menu, string displayText)
    {
        this.level = level;
        this.menu = menu;

        if (label == null)
            label = GetComponentInChildren<TMP_Text>();

        if (targetImage == null)
            targetImage = GetComponent<Image>();

        if (button == null)
            button = GetComponent<Button>();

        if (label != null)
            label.text = displayText;

        RefreshVisuals();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => menu.ChooseLevel(this.level));
        }
    }

    public void RefreshVisuals()
    {
        if (targetImage == null) return;

        bool completed = GameManager.Instance != null && GameManager.Instance.IsLevelCompleted(level);

        if (completed && completedSprite != null)
            targetImage.sprite = completedSprite;
        else if (defaultSprite != null)
            targetImage.sprite = defaultSprite;
    }
}