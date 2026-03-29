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
    public GameObject foxPrefab;

    [Header("Spawn Area")]
    public float spawnXMin = -8f;
    public float spawnXMax = 8f;
    public float spawnY = 10f;

    [Header("Spawn Rate (detik)")]
    public float baseSpawnRate = 1.2f;
    public float foxSpawnInterval = 8f;    // Siang: jarang
    public float foxSpawnIntervalAfternoon = 4f; // Sore: sering

    [Header("Probability")]
    [Range(0, 1)] public float frozenCarrotChance = 0.2f;
    [Range(0, 1)] public float snowCarrotChance = 0.4f; // Salju: wortel salju sering

    private WeatherManager weatherManager;
    private GameManager gameManager;
    private bool isSpawning = false;
    private Coroutine spawnCoroutine;
    private Coroutine foxCoroutine;

    void Start()
    {
        weatherManager = FindObjectOfType<WeatherManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void StartSpawning()
    {
        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnLoop());
        foxCoroutine = StartCoroutine(FoxSpawnLoop());
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
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
        float rate = baseSpawnRate;
        if (weatherManager == null) return rate;

        switch (weatherManager.CurrentWeather)
        {
            case WeatherType.Snow:
                rate *= 0.8f; // Salju: spawn lebih cepat
                break;
            case WeatherType.AfternoonDry:
                rate *= 0.9f;
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
        if (foxPrefab == null) return;

        // Rubah muncul di posisi random X, tidak jatuh (Y tetap)
        float x = Random.Range(spawnXMin + 2f, spawnXMax - 2f);
        float y = Random.Range(-2f, 3f); // muncul di area tengah layar
        Vector3 spawnPos = new Vector3(x, y, 0f);

        Instantiate(foxPrefab, spawnPos, Quaternion.identity);
        Debug.Log($"[SpawnManager] Rubah muncul di {spawnPos}");
    }
}
