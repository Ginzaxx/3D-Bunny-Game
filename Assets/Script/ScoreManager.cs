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
    // Score 61  -> AfternoonDry
    // Score 121 -> Snow
    public int afternoonThreshold = 5;
    public int snowThreshold = 10;

    public int CurrentScore { get; private set; } = 0;
    public int CarrotCount  { get; private set; } = 0;

    private int highScore = 0;
    private const string HIGH_SCORE_KEY = "HighScore";

    private WeatherManager weatherManager;

    void Start()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        weatherManager = FindObjectOfType<WeatherManager>();
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
    /// Nilai negatif untuk penalti (misal rubah terlewat: -20).
    /// </summary>
    public void AddCarrot(int amount)
    {
        CarrotCount += amount;
        if (CarrotCount < 0) CarrotCount = 0;   // tidak bisa minus
        UpdateUI();
    }

    // ─── Weather Threshold ────────────────────────────────────────────────────

    /// <summary>
    /// Cek apakah score sudah mencapai threshold pergantian cuaca.
    /// Dipanggil setiap kali score bertambah.
    /// </summary>
    void CheckWeatherThreshold()
    {
        if (weatherManager == null) return;

        if (CurrentScore >= snowThreshold)
        {
            if (weatherManager.CurrentWeather != WeatherType.Snow)
                weatherManager.ChangeWeather(WeatherType.Snow);
        }
        else if (CurrentScore >= afternoonThreshold)
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