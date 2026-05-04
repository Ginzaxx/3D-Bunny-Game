using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// SpawnManager - Mengatur spawn objek jatuh (wortel & rubah)
/// Pola spawn berubah sesuai cuaca
/// Attach ke GameObject SpawnManager
/// </summary>
public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject carrotNormalPrefab;
    public GameObject carrotFrozenPrefab;
    public GameObject kitObject;
    public GameObject foxPrefab;

    [Header("Spawn Area")]
    public float spawnXMin = -8f;
    public float spawnXMax = 8f;
    public float spawnY = 10f;

    [Header("Spawn Rate (detik)")]
    public float baseSpawnRate = 1.5f;     // Waktu spawn awal (paling lambat)
    public float minSpawnRate = 0.7f;      // Waktu spawn tercepat (saat mencapai target)
    public int targetCarrotsForMaxSpeed = 90; // Target wortel agar kecepatan spawn maksimal
    [Space]
    public float kitSpawnRate = 20f;
    public float foxSpawnInterval = 8f;    // Siang: jarang
    public float foxSpawnIntervalAfternoon = 4f; // Sore: sering

    [Header("Probability")]
    [Range(0, 1)] public float frozenCarrotChance = 0.2f;
    [Range(0, 1)] public float snowCarrotChance = 0.4f; // Salju: wortel salju sering

    [Header("Pooling Settings")]
    public int foxPoolSize = 5;
    private List<GameObject> foxPool = new List<GameObject>();

    private WeatherManager weatherManager;
    private GameManager gameManager;
    private ScoreManager scoreManager;
    private CookingManager cookingManager;

    private bool isSpawning = false;
    private Coroutine spawnCoroutine;
    private Coroutine kitCoroutine;
    private Coroutine foxCoroutine;

    void Start()
    {
        weatherManager = FindObjectOfType<WeatherManager>();
        gameManager = FindObjectOfType<GameManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        cookingManager = FindObjectOfType<CookingManager>();
        
        InitializeFoxPool();
    }

    void InitializeFoxPool()
    {
        if (foxPrefab == null) return;
        
        for (int i = 0; i < foxPoolSize; i++)
        {
            GameObject obj = Instantiate(foxPrefab);
            obj.SetActive(false);
            foxPool.Add(obj);
        }
    }

    public GameObject GetFoxFromPool()
    {
        foreach (GameObject fox in foxPool)
        {
            if (!fox.activeInHierarchy)
            {
                return fox;
            }
        }

        // Opsional: Expand pool jika semua sedang dipakai
        GameObject newFox = Instantiate(foxPrefab);
        newFox.SetActive(false);
        foxPool.Add(newFox);
        return newFox;
    }

    public void StartSpawning()
    {
        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnLoop());
        kitCoroutine = StartCoroutine(KitSpawnLoop());
        foxCoroutine = StartCoroutine(FoxSpawnLoop());
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        if (kitCoroutine != null) StopCoroutine(kitCoroutine);
        if (foxCoroutine != null) StopCoroutine(foxCoroutine);
    }

    IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            float rate = GetSpawnRate();
            yield return new WaitForSeconds(rate);
            SpawnCarrot();
        }
    }

    IEnumerator KitSpawnLoop()
    {
        while (isSpawning)
        {
            float rate = GetKitSpawnRate();
            yield return new WaitForSeconds(rate);

            SpawnKit();
        }
    }

    IEnumerator FoxSpawnLoop()
    {
        // Delay awal sebelum rubah pertama
        yield return new WaitForSeconds(5f);

        while (isSpawning)
        {
            float interval = GetFoxInterval();
            yield return new WaitForSeconds(interval);

            if (isSpawning)
                SpawnFox();
        }
    }

    float GetSpawnRate()
    {
        float currentRate = baseSpawnRate;

        if (scoreManager != null)
        {
            // Hitung persentase (0.0 sampai 1.0)
            // Mathf.Clamp01 memastikan nilainya tidak lebih dari 1 (100%) meskipun wortel > 90
            float progress = Mathf.Clamp01((float)scoreManager.CarrotCount / targetCarrotsForMaxSpeed);
            
            // Semakin besar progress, currentRate akan semakin mendekati minSpawnRate (semakin cepat)
            currentRate = Mathf.Lerp(baseSpawnRate, minSpawnRate, progress);
        }

        return currentRate;
    }

    float GetKitSpawnRate()
    {
        float rate = kitSpawnRate;
        if (weatherManager == null) return rate;

        switch (weatherManager.CurrentWeather)
        {
            case WeatherType.Snow:
                rate *= 0.6f;
                break;
            case WeatherType.AfternoonDry:
                rate *= 0.8f;
                break;
        }
        return rate;
    }

    float GetFoxInterval()
    {
        if (weatherManager == null) return foxSpawnInterval;

        return weatherManager.CurrentWeather == WeatherType.AfternoonDry
            ? foxSpawnIntervalAfternoon
            : foxSpawnInterval;
    }

    void SpawnCarrot()
    {
        float x = Random.Range(spawnXMin, spawnXMax);
        Vector3 spawnPos = new Vector3(x, spawnY, 0f);

        GameObject prefab = ChooseCarrotPrefab();
        if (prefab != null)
            Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    GameObject ChooseCarrotPrefab()
    {
        float roll = Random.value;

        // Cuaca salju: wortel salju (beku) lebih sering
        float frozenChance = frozenCarrotChance;
        if (weatherManager != null && weatherManager.CurrentWeather == WeatherType.Snow)
            frozenChance = snowCarrotChance;

        if (roll < frozenChance && carrotFrozenPrefab != null)
            return carrotFrozenPrefab;

        return carrotNormalPrefab;
    }

    void SpawnFox()
    {
        GameObject fox = GetFoxFromPool();
        if (fox == null) return;

        // Rubah muncul di posisi random X, tidak jatuh (Y tetap)
        float x = Random.Range(spawnXMin + 2f, spawnXMax - 2f);
        float y = Random.Range(-2f, 3f); // muncul di area tengah layar
        Vector3 spawnPos = new Vector3(x, y, 0f);

        fox.transform.position = spawnPos;
        fox.transform.rotation = Quaternion.identity;
        fox.SetActive(true);

        AudioManager.Instance?.PlayFoxAppear();
        Debug.Log($"[SpawnManager] Rubah (Pooled) muncul di {spawnPos}");
    }

    void SpawnKit()
    {
        if (kitObject == null) return;

        kitObject.SetActive(true);
        cookingManager.SetIndexRequest();

        Debug.Log("[SpawnManager] Kit has spawned");
    }
}
