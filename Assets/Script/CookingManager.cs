using System.Collections;
using TMPro;
using UnityEngine;

public enum CookStage
{
    Raw,
    Boiled,
    Grilled,
    Chopped
}

public class CookingManager : MonoBehaviour
{
    [Header("Cooking Prefab List")]
    [SerializeField] private GameObject CookingObject;
    [SerializeField] private GameObject RawPrefab;      // Index 1
    [SerializeField] private GameObject ChoppedPrefab;  // Index 2
    [SerializeField] private GameObject GrilledPrefab;  // Index 3
    [SerializeField] private GameObject BoiledPrefab;   // Index 4
    [SerializeField] private TextMeshProUGUI indexText;

    [Header("Food Variables")]
    [SerializeField] private int indexRequest;
    [SerializeField] private int indexCooking;
    [SerializeField] private bool hasRequest;
    [SerializeField] private bool isCooking;
    [SerializeField] private bool isCooked;
    [SerializeField] private CookStage cookStage;

    [Header("References")]
    [SerializeField] private WeatherManager weatherManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameObject kitObject;

    [Header("Spawn Rate (detik)")]
    public float kitSpawnRate = 20f;
    private bool isSpawning = false;
    private Coroutine kitCoroutine;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        weatherManager = FindObjectOfType<WeatherManager>();
    }

    public void StartSpawning()
    {
        isSpawning = true;
        kitCoroutine = StartCoroutine(KitSpawnLoop());
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (kitCoroutine != null) StopCoroutine(kitCoroutine);
    }

    IEnumerator KitSpawnLoop()
    {
        while (isSpawning)
        {
            float rate = GetKitSpawnRate();
            yield return new WaitForSeconds(rate);
            DespawnKit();

            yield return new WaitForSeconds(5f);
            SpawnKit();
        }
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

    void SpawnKit()
    {
        SetRequest();

        Debug.Log("[SpawnManager] Kit has spawned");
    }

    void DespawnKit()
    {
        ResetRequest();

        Debug.Log("[SpawnManager] Kit has despawned");
    }

    private void UpdateCooking()
    {
        if (CookingObject == null) return;

        switch(indexCooking)
        {
            case 0:
                CookingObject = null;
                break;
            case 1:
                CookingObject = RawPrefab;
                break;
            case 2:
                CookingObject = ChoppedPrefab;
                break;
            case 3:
                CookingObject = GrilledPrefab;
                break;
            case 4:
                CookingObject = BoiledPrefab;
                break;
        }

        indexText.text = $"{indexCooking}";

        Debug.Log("[CookingManager] New Cooking Index : " + indexCooking);
    }

    public void SetIndexCooking(string method)
    {
        switch (method)
        {
        case "Board":
            cookStage = CookStage.Chopped;
            indexCooking = 2;
            break;
        case "Grill":
            cookStage = CookStage.Grilled;
            indexCooking = 3;
            break;
        case "Pot":
            cookStage = CookStage.Boiled;
            indexCooking = 4;
            break;
        }

        UpdateCooking();
    }

    public void StartCooking()
    {
        isCooking = true;
        isCooked = true;
    }

    public void TakeCooking()
    {
        isCooking = false;
    }

    public void SetRequest()
    {
        if (kitObject != null && !kitObject.activeSelf)
            kitObject.SetActive(true);

        indexRequest = Random.Range(2, 4);
        indexCooking = 1;
        isCooked = false;

        Debug.Log("[CookingManager] Request Index : " + indexRequest);
    }

    public void ResetRequest()
    {
        if (kitObject != null && kitObject.activeSelf)
            kitObject.SetActive(false);

        if (indexRequest == indexCooking)
            scoreManager.AddScore(50);
        else
            scoreManager.AddScore(-25);

        indexRequest = 0;
        indexCooking = 0;

        UpdateCooking();
    }

    public void TakeRequest()
    {
        hasRequest = true;
    }

    public void FinishRequest()
    {
        hasRequest = false;

        ResetRequest();
    }

    public int G_IndexRequest => indexRequest;
    public int G_IndexCooking => indexCooking;
    public bool G_HasRequest => hasRequest;
    public bool G_IsCooking => isCooking;
    public bool G_IsCooked => isCooked;
}