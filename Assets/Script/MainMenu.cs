using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    [Header("Scene (Drag dari Inspector)")]
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneToLoad;
#endif

    private string sceneName;

    private void Awake()
    {
#if UNITY_EDITOR
        if (sceneToLoad != null)
        {
            sceneName = sceneToLoad.name;
        }
#endif
    }

    private void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
    }

    public void StartGame()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene belum di-assign di Inspector!");
        }
    }

    public void ExitGame()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();
    }
}