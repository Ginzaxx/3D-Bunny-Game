using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// TimerManager - Countdown timer utama game
/// Timer berkurang seiring waktu, jika habis = kalah
/// Attach ke GameObject Timer
/// </summary>
public class TimerManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startTime = 60f;
    public float currentTime;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public UnityEvent onTimerEnd;

    [Header("Visual Warning")]
    public Color normalColor = Color.white;
    public Color warningColor = Color.yellow;
    public Color dangerColor = Color.red;
    public float warningThreshold = 20f;
    public float dangerThreshold = 10f;

    private bool isRunning = false;
    private bool isGameOver = false;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        ResetTimer();
    }

    void Update()
    {
        if (!isRunning || isGameOver) return;

        currentTime -= Time.deltaTime;
        currentTime = Mathf.Max(0, currentTime);

        UpdateUI();

        if (currentTime <= 0)
        {
            OnTimerEnd();
        }
    }

    public void StartTimer()
    {
        isRunning = true;
        isGameOver = false;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = startTime;
        isGameOver = false;
        UpdateUI();
    }

    /// <summary>
    /// Tambah atau kurangi waktu (positif = tambah, negatif = kurangi)
    /// </summary>
    public void AddTime(float seconds)
    {
        currentTime += seconds;
        currentTime = Mathf.Max(0, currentTime);

        // Efek visual flash
        if (timerText != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashTimer(seconds > 0));
        }
    }

    void UpdateUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Warna berdasarkan sisa waktu
        if (currentTime <= dangerThreshold)
            timerText.color = dangerColor;
        else if (currentTime <= warningThreshold)
            timerText.color = warningColor;
        else
            timerText.color = normalColor;
    }

    void OnTimerEnd()
    {
        isGameOver = true;
        isRunning = false;
        onTimerEnd?.Invoke();
        gameManager?.GameOver();
    }

    System.Collections.IEnumerator FlashTimer(bool isBonus)
    {
        Color flashColor = isBonus ? Color.green : Color.red;
        timerText.color = flashColor;
        yield return new WaitForSeconds(0.3f);
        UpdateUI();
    }

    public float CurrentTime => currentTime;
    public bool IsRunning => isRunning;
}
