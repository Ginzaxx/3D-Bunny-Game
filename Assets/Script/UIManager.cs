using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UIManager - Mengatur semua panel UI game
/// Attach ke GameObject UIManager
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject hudPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("HUD Elements")]
    public TextMeshProUGUI scoreHUD;
    public TextMeshProUGUI timerHUD;
    public TextMeshProUGUI weatherHUD;

    [Header("GameOver Elements")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI gameOverMessage;

    [Header("Menu Elements")]
    public Button playButton;
    public Button[] weatherButtons; // [0]=Siang, [1]=Sore, [2]=Salju

    [Header("Weather Icons")]
    public Image weatherIcon;
    public Sprite sunSprite;
    public Sprite afternoonSprite;
    public Sprite snowSprite;

    private GameManager gameManager;
    private WeatherManager weatherManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        weatherManager = FindObjectOfType<WeatherManager>();

        // Setup tombol
        playButton?.onClick.AddListener(() => gameManager?.StartGame());

        // Setup weather buttons
        if (weatherButtons.Length >= 3)
        {
            weatherButtons[0].onClick.AddListener(() => { weatherManager?.SetDayDry(); UpdateWeatherUI(); });
            weatherButtons[1].onClick.AddListener(() => { weatherManager?.SetAfternoonDry(); UpdateWeatherUI(); });
            weatherButtons[2].onClick.AddListener(() => { weatherManager?.SetSnow(); UpdateWeatherUI(); });
        }

        ShowMenu();
    }

    // ============================
    //   Panel Controls
    // ============================

    public void ShowMenu()
    {
        SetAllPanels(false);
        if (menuPanel != null) menuPanel.SetActive(true);
    }

    public void ShowHUD()
    {
        SetAllPanels(false);
        if (hudPanel != null) hudPanel.SetActive(true);
        UpdateWeatherUI();
    }

    public void ShowPause()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ShowGameOver(int finalScore)
    {
        SetAllPanels(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = $"Score: {finalScore}";

        ScoreManager sm = FindObjectOfType<ScoreManager>();
        if (highScoreText != null && sm != null)
            highScoreText.text = $"Best: {sm.HighScore}";

        if (gameOverMessage != null)
        {
            string[] messages = {
                "Waktu Habis! 🥕",
                "Kelinci Kelelahan! 😅",
                "Coba Lagi! 🐰"
            };
            gameOverMessage.text = messages[Random.Range(0, messages.Length)];
        }
    }

    void SetAllPanels(bool active)
    {
        if (menuPanel != null) menuPanel.SetActive(active);
        if (hudPanel != null) hudPanel.SetActive(active);
        if (pausePanel != null) pausePanel.SetActive(active);
        if (gameOverPanel != null) gameOverPanel.SetActive(active);
    }

    // ============================
    //   Weather UI
    // ============================

    void UpdateWeatherUI()
    {
        if (weatherManager == null) return;

        string weatherName = "";
        Sprite icon = null;

        switch (weatherManager.CurrentWeather)
        {
            case WeatherType.DayDry:
                weatherName = "☀️ Siang Kering";
                icon = sunSprite;
                break;
            case WeatherType.AfternoonDry:
                weatherName = "🌅 Sore Kering";
                icon = afternoonSprite;
                break;
            case WeatherType.Snow:
                weatherName = "❄️ Salju";
                icon = snowSprite;
                break;
        }

        if (weatherHUD != null) weatherHUD.text = weatherName;
        if (weatherIcon != null && icon != null) weatherIcon.sprite = icon;
    }

    // ============================
    //   Button Handlers (untuk Inspector)
    // ============================

    public void OnPauseButtonPressed()
    {
        gameManager?.PauseGame();
    }

    public void OnResumeButtonPressed()
    {
        gameManager?.ResumeGame();
    }

    public void OnRestartButtonPressed()
    {
        gameManager?.RestartGame();
    }

    public void OnMainMenuButtonPressed()
    {
        Time.timeScale = 1f;
        gameManager?.ShowMenu();
    }

    public void OnQuitButtonPressed()
    {
        gameManager?.QuitGame();
    }
}
