using UnityEngine;
using TMPro;

/// <summary>
/// ScoreManager - Mengatur skor dan jumlah wortel pemain
/// Attach ke GameObject ScoreManager
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("UI - Score")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    [Header("UI - Carrot")]
    public TextMeshProUGUI carrotText;  // Menampilkan jumlah wortel

    [Header("Weather Thresholds")]
    public int afternoonThreshold = 25; // 25 carrots
    public int snowThreshold = 50;      // 50 carrots
    public int targetCarrots = 75;

    [field: SerializeField] public int CurrentScore { get; private set; } = 0;
    [field: SerializeField] public int CarrotCount  { get; private set; } = 0;

    private int highScore = 0;
    private const string HIGH_SCORE_KEY = "HighScore";

    private WeatherManager weatherManager;
    private GameManager gameManager;

    void Start()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        weatherManager = FindObjectOfType<WeatherManager>();
        gameManager = FindObjectOfType<GameManager>();
        ResetScore();
    }

    // ─── Score ────────────────────────────────────────────────────────────────

    public void AddScore(int points)
    {
        CurrentScore += points;
        if (CurrentScore < 0) CurrentScore = 0;
        UpdateUI();
        CheckHighScore();
        CheckWeatherThreshold();
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        CarrotCount  = 0;
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

    // ─── Carrot ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Tambah atau kurangi jumlah wortel.
    /// Nilai negatif untuk penalti.
    /// </summary>
    public void AddCarrot(int amount)
    {
        CarrotCount += amount;
        if (CarrotCount < 0) CarrotCount = 0;   // tidak bisa minus
        UpdateUI();
        CheckWinCondition();
        CheckWeatherThreshold();
    }

    void CheckWinCondition()
    {
        if (CarrotCount >= targetCarrots && gameManager != null && gameManager.CurrentState == GameState.Playing)
        {
            gameManager.WinGame();
        }
    }

    // ─── Weather Threshold ────────────────────────────────────────────────────

    /// <summary>
    /// Cek apakah jumlah wortel sudah mencapai threshold pergantian cuaca.
    /// </summary>
    void CheckWeatherThreshold()
    {
        if (weatherManager == null) return;

        if (CarrotCount >= snowThreshold)
        {
            if (weatherManager.CurrentWeather != WeatherType.Snow)
                weatherManager.ChangeWeather(WeatherType.Snow);
        }
        else if (CarrotCount >= afternoonThreshold)
        {
            if (weatherManager.CurrentWeather != WeatherType.AfternoonDry)
                weatherManager.ChangeWeather(WeatherType.AfternoonDry);
        }
    }

    // ─── UI ───────────────────────────────────────────────────────────────────

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {CurrentScore}";

        if (highScoreText != null)
            highScoreText.text = $"Best: {highScore}";

        if (carrotText != null)
            carrotText.text = $"Carrot: {CarrotCount}";
    }

    // ─── Getters ──────────────────────────────────────────────────────────────

    public int HighScore => highScore;
}