using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver
}

/// <summary>
/// GameManager - Singleton mengatur state game keseluruhan
/// Attach ke GameObject GameManager
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    public TimerManager timerManager;
    public SpawnManager spawnManager;
    public ScoreManager scoreManager;
    public WeatherManager weatherManager;
    public UIManager uiManager;

    [Header("Camera")]
    public CameraController cameraController;

    public GameState CurrentState { get; private set; } = GameState.Menu;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartGame();
    }

    public void ShowMenu()
    {
        CurrentState = GameState.Menu;
        uiManager?.ShowMenu();
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        scoreManager?.ResetScore();
        timerManager?.ResetTimer();
        timerManager?.StartTimer();
        spawnManager?.StartSpawning();
        cameraController?.SetGameplayCamera();
        uiManager?.ShowHUD();
        ClearAllFallingObjects();
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;
        CurrentState = GameState.Paused;
        Time.timeScale = 0f;
        timerManager?.StopTimer();
        uiManager?.ShowPause();
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        timerManager?.StartTimer();
        uiManager?.ShowHUD();
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        spawnManager?.StopSpawning();
        int finalScore = scoreManager != null ? scoreManager.CurrentScore : 0;
        uiManager?.ShowGameOver(finalScore);
        Debug.Log($"[GameManager] Game Over! Score: {finalScore}");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        ClearAllFallingObjects();
        StartGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void ClearAllFallingObjects()
    {
        // Hapus semua wortel/rubah yang ada di scene
        FallingObject[] objects = FindObjectsOfType<FallingObject>();
        foreach (var obj in objects)
            Destroy(obj.gameObject);
    }
}
