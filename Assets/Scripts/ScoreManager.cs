using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TMP_Text scoreText;

    public int Score { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        RefreshUI();
    }

    public void AddPoints(int amount)
    {
        Score += amount;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {Score}";
    }

    public void ResetScore()
    {
        Score = 0;
        RefreshUI();
    }
}