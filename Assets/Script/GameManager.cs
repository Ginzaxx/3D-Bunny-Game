using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
public class GameManager : MonoBehaviour, ISerializationCallbackReceiver
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    public TimerManager timerManager;
    public SpawnManager spawnManager;
    public ScoreManager scoreManager;
    public WeatherManager weatherManager;
    public CookingManager cookingManager;
    public UIManager uiManager;

    [Header("Camera")]
    public CameraController cameraController;

    [Header("Next Level Scene (Drag dari Inspector)")]
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneToLoad;
#endif

    [HideInInspector]
    [SerializeField] private string sceneName;

    public GameState CurrentState { get; private set; } = GameState.Menu;

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        if (sceneToLoad != null)
        {
            sceneName = sceneToLoad.name;
        }
        else
        {
            sceneName = null;
        }
#endif
    }

    public void OnAfterDeserialize()
    {
    }

    void Awake()
    {
        Instance = this;
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
        cookingManager?.StartSpawning();
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

    public void WinGame()
    {
        CurrentState = GameState.GameOver;
        timerManager?.StopTimer();
        spawnManager?.StopSpawning();
        cookingManager?.StopSpawning();
        int finalScore = scoreManager != null ? scoreManager.CurrentScore : 0;
        uiManager?.ShowWin(finalScore);
        AudioManager.Instance?.PlayWin();
        Debug.Log($"[GameManager] Game Won! Score: {finalScore}");

        if (!string.IsNullOrEmpty(sceneName))
        {
            StartCoroutine(LoadNextLevelAfterDelay(3f));
        }
        else
        {
            Debug.LogWarning("Scene berikutnya belum di-assign di GameManager Inspector!");
        }
    }

    private System.Collections.IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        spawnManager?.StopSpawning();
        cookingManager?.StopSpawning();
        int finalScore = scoreManager != null ? scoreManager.CurrentScore : 0;
        uiManager?.ShowGameOver(finalScore);
        AudioManager.Instance?.PlayLose();
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
