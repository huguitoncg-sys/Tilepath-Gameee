using TMPro;
using UnityEngine;

public class LevelHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelChanged += UpdateLevelText;
        }

        UpdateLevelText();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnLevelChanged -= UpdateLevelText;
    }

    public void UpdateLevelText()
    {
        if (levelText == null) return;

        int levelNumber = 1;
        if (GameManager.Instance != null)
        {
            int idx = GameManager.Instance.GetSelectedLevelIndex();
            if (idx >= 0) levelNumber = idx + 1;
        }

        levelText.text = $"Level: {levelNumber}";
    }
}