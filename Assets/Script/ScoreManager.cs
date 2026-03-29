using UnityEngine;
using TMPro;

/// <summary>
/// ScoreManager - Mengatur skor pemain
/// Attach ke GameObject ScoreManager
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    public int CurrentScore { get; private set; } = 0;

    private int highScore = 0;
    private const string HIGH_SCORE_KEY = "HighScore";

    void Start()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        ResetScore();
    }

    public void AddScore(int points)
    {
        CurrentScore += points;
        UpdateUI();
        CheckHighScore();
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        UpdateUI();
    }

    void CheckHighScore()
    {
        if (CurrentScore > highScore)
        {
            highScore = CurrentScore;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {CurrentScore}";

        if (highScoreText != null)
            highScoreText.text = $"Best: {highScore}";
    }

    public int HighScore => highScore;
}
