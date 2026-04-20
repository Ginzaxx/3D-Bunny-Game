using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    void Update()
    {
        // Cek apakah gameOverPanel sedang aktif
        if (gameOverPanel != null && gameOverPanel.activeInHierarchy)
        {
            // Jika menekan Enter (Return) atau Escape
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            {
                // Memanggil fungsi yang sudah ada untuk kembali ke menu
                OnMainMenuButtonPressed();
            }
        }
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
            gameOverMessage.text = "Waktu Habis!";
        }
    }

    public void ShowWin(int finalScore)
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
            gameOverMessage.text = "Selamat! Kamu Menang!";
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
        // Pastikan waktu berjalan normal kembali (penting jika game di-pause)
        Time.timeScale = 1f;

        // Ganti "MainMenu" dengan nama scene menu utamamu yang ada di Build Settings
        SceneManager.LoadScene("Main Menu"); 
        
        // Opsional: Jika kamu punya fungsi di GameManager untuk reset state
        // gameManager?.ShowMenu(); 
    }

    public void OnQuitButtonPressed()
    {
        gameManager?.QuitGame();
    }
}
