using UnityEngine;

public enum FallingObjectType
{
    CarrotNormal,   // Wortel Biasa (kuning) - nambah timer
    CarrotFrozen,   // Wortel Beku (biru)    - ngurangi timer
    Fox             // Rubah - harus di-click tepat waktu
}

/// <summary>
/// FallingObject - Base class untuk semua objek yang jatuh
/// Attach ke prefab Wortel / Rubah
/// </summary>
public class FallingObject : MonoBehaviour
{
    [Header("Type")]
    public FallingObjectType objectType = FallingObjectType.CarrotNormal;

    [Header("Fall Settings")]
    public float fallSpeed = 4f;
    public float maxFallSpeed = 8f; // Kecepatan maksimal (~1.5 detik untuk jarak 20 unit)
    public float destroyY = -10f;

    [Header("Timer Effect")]
    public float timerBonus = 5f;     // detik ditambah saat nangkep wortel normal
    public float timerPenalty = 3f;   // detik dikurangi saat nangkep wortel beku

    [Header("Fox Settings")]
    public float foxClickWindow = 2f;   // detik window untuk click rubah
    public float foxZOffset = 5f;      // jarak Z di belakang player saat spawn
    public int foxMissCarrotPenalty = 2; // carrot dikurangi jika rubah tidak di-click
    public GameObject foxWarningVFX;    // efek peringatan rubah

    // State
    private bool isCaught = false;
    private bool isMissed = false;
    private float foxTimer = 0f;
    private bool foxWindowOpen = false;

    // References
    private TimerManager timerManager;
    private ScoreManager scoreManager;
    private GameManager gameManager;
    private WeatherManager weatherManager;

    void Start()
    {
        timerManager  = FindObjectOfType<TimerManager>();
        scoreManager  = FindObjectOfType<ScoreManager>();
        gameManager   = FindObjectOfType<GameManager>();
        weatherManager = FindObjectOfType<WeatherManager>();

        AdjustSpeedForWeather();

        if (objectType == FallingObjectType.Fox)
            InitFox();
    }

    void AdjustSpeedForWeather()
    {
        if (weatherManager == null) return;

        switch (weatherManager.CurrentWeather)
        {
            case WeatherType.Snow:
                fallSpeed *= 1.3f;
                maxFallSpeed *= 1.5f; // Jangan lupa update max speed untuk salju
                break;
            case WeatherType.AfternoonDry:
                fallSpeed *= 1.1f;
                maxFallSpeed *= 1.3f; // Jangan lupa update max speed untuk sore
                break;
        }
    }

    void InitFox()
    {
        // Reset rotasi supaya fox tidak terbalik saat spawn
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        // Posisikan fox jauh di belakang player
        Vector3 pos = transform.position;
        pos.z += foxZOffset;
        transform.position = pos;

        foxWindowOpen = true;
        foxTimer = foxClickWindow;
        if (foxWarningVFX != null) foxWarningVFX.SetActive(true);
    }

    void Update()
    {
        if (isCaught || isMissed) return;

        if (objectType == FallingObjectType.Fox)
        {
            HandleFoxBehavior();
            return;
        }

        // Hitung kecepatan jatuh dinamis berdasarkan jumlah carrot
        float currentSpeed = fallSpeed;
        if (scoreManager != null)
        {
            // Lerp kecepatan dari kecepatan awal ke kecepatan maksimal (13.3f)
            float progress = (float)scoreManager.CarrotCount / scoreManager.targetCarrots;
            currentSpeed = Mathf.Lerp(fallSpeed, maxFallSpeed, progress);
        }

        // Jatuh ke bawah
        transform.Translate(Vector3.down * currentSpeed * Time.deltaTime);

        // Lewat batas bawah = missed
        if (transform.position.y < destroyY)
        {
            OnMissed();
        }
    }

    void HandleFoxBehavior()
    {
        if (foxWindowOpen)
        {
            foxTimer -= Time.deltaTime;
            if (foxTimer <= 0f)
            {
                FoxMissed();
            }
        }
    }

    void OnMouseDown()
    {
        if (objectType == FallingObjectType.Fox && foxWindowOpen && !isCaught)
        {
            FoxClicked();
        }
    }

    public void FoxClicked()
    {
        if (isCaught) return;
        isCaught = true;
        foxWindowOpen = false;
        scoreManager?.AddScore(10);
        AudioManager.Instance?.PlayFoxClick();
        Destroy(gameObject, 0.3f);
    }

    void FoxMissed()
    {
        foxWindowOpen = false;
        isMissed = true;

        // Penalti disesuaikan untuk target 75 carrot
        timerManager?.AddTime(-15f);
        scoreManager?.AddCarrot(-10);
        scoreManager?.AddScore(-50);
        AudioManager.Instance?.PlayFoxMissed();

        Destroy(gameObject, 0.5f);
    }

    public void OnCaught()
    {
        if (isCaught) return;
        isCaught = true;

        switch (objectType)
        {
            case FallingObjectType.CarrotNormal:
                float progress = 0f;
                if (scoreManager != null) progress = (float)scoreManager.CarrotCount / scoreManager.targetCarrots;
                float dynamicBonus = 3f - (0.5f * progress);
                
                timerManager?.AddTime(dynamicBonus);
                scoreManager?.AddCarrot(1);
                scoreManager?.AddScore(5);
                AudioManager.Instance?.PlayCatchNormal();
                break;

            case FallingObjectType.CarrotFrozen:
                if (timerManager != null)
                {
                    float penalty = 5f + timerManager.CurrentTime * 0.2f;
                    timerManager.AddTime(-penalty);
                }
                AudioManager.Instance?.PlayCatchFrozen();
                break;
        }

        Destroy(gameObject, 0.1f);
    }

    void OnMissed()
    {
        isMissed = true;
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCaught || isMissed) return;

        if (other.CompareTag("Basket"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null && player.TryCatch(this))
            {
                OnCaught();
            }
        }

        if (other.CompareTag("Ground"))
        {
            OnMissed();
        }
    }
}