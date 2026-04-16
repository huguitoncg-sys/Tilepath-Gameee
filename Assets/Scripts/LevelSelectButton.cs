using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    public TMP_Text label;

    private LevelData level;
    private MenuUIController menu;

    public void Setup(LevelData level, MenuUIController menu, string displayText)
    {
        this.level = level;
        this.menu = menu;

        if (label == null)
            label = GetComponentInChildren<TMP_Text>();

        if (label != null)
            label.text = displayText;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => menu.ChooseLevel(this.level));
    }
}