using System.Collections;
using TMPro;
using UnityEngine;

public class LevelHUD : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TMP_Text levelText;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelChanged += UpdateLevelText;
        }

        UpdateLevelText();
        StartCoroutine(UpdateTextNextFrame());
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelChanged -= UpdateLevelText;
        }
    }

    private IEnumerator UpdateTextNextFrame()
    {
        yield return null;
        UpdateLevelText();
    }

    public void UpdateLevelText()
    {
        if (levelText == null)
        {
            Debug.LogWarning("LevelHUD is missing its TMP_Text reference.");
            return;
        }

        if (GameManager.Instance == null)
        {
            levelText.text = "Level:\n?";
            return;
        }

        int levelNumber = GameManager.Instance.GetSelectedLevelNumber();

        if (levelNumber <= 0)
        {
            levelText.text = "Level:\n?";
            return;
        }

        levelText.text = $"Level:\n{levelNumber}";

        Debug.Log("HUD displaying selected level: " + levelNumber);
    }
}