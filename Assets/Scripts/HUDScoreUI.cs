using TMPro;
using UnityEngine;

public class HUDScoreUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Score")]
    [SerializeField] private int score = 0;

    public int Score => score;

    private void Awake()
    {
        if (scoreText == null)
            Debug.LogError("[HUDScoreUI] scoreText is NOT assigned in the Inspector.", this);

        Refresh();
        Debug.Log($"[HUDScoreUI] Awake. Starting Score = {score}", this);
    }

    public void ResetScore()
    {
        score = 0;
        Debug.Log("[HUDScoreUI] Score reset to 0", this);
        Refresh();
    }

    public void AddScore(int amount = 1)
    {
        if (amount == 0) return;

        score += amount;
        if (score < 0) score = 0;

        Debug.Log($"[HUDScoreUI] AddScore({amount}) -> Score = {score}", this);
        Refresh();
    }

    private void Refresh()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
}