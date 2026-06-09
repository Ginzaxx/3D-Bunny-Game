using System.Collections;
using UnityEngine;

public enum CookStage
{
    Request,
    Chopped,
    Grilled,
    Boiled
}

public class CookingManager : MonoBehaviour
{
    [Header("Cooking Objects")]
    [SerializeField] private GameObject[] requestObjects;
    [SerializeField] private GameObject[] cookingObjects;

    [Header("Food Variables")]
    [SerializeField] private int requestIndex;
    [SerializeField] private int cookingIndex;
    [SerializeField] private bool hasRequest;
    [SerializeField] private bool hasCooked;
    [SerializeField] private bool isCooking;

    [Header("References")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private GameObject kitObject;

    [Header("Spawn Rate")]
    [SerializeField] private float kitSpawnRate = 20f;
    [SerializeField] private float kitDespawnRate = 5f;
    [SerializeField] private bool isSpawning;
    [SerializeField] private int spawnCycle;

    private void Start()
    {
        if (scoreManager == null) scoreManager = FindObjectOfType<ScoreManager>();
        if (timerManager == null) timerManager = FindObjectOfType<TimerManager>();

        foreach (var obj in requestObjects)
            obj.SetActive(false);
        foreach (var obj in cookingObjects)
            obj.SetActive(false);
        kitObject.SetActive(false);
    }

    public void StartSpawning()
    {
        isSpawning = true;
        StartCoroutine(DespawnKit());
        AudioManager.Instance.PlaySnowMusic();
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    private IEnumerator SpawnKit()
    {
        int spawnCycleKit = spawnCycle;
        SetRequest();
        Debug.Log("[CookingManager] Kit has spawned");
        yield return new WaitForSeconds(kitSpawnRate);
        if (spawnCycle == spawnCycleKit) StartCoroutine(DespawnKit());
    }

    private IEnumerator DespawnKit()
    {
        spawnCycle += 1;
        ResetRequest();
        Debug.Log("[CookingManager] Kit has despawned");
        yield return new WaitForSeconds(kitDespawnRate);
        if (isSpawning) StartCoroutine(SpawnKit());
    }

    public void StartRequest()
    {
        hasRequest = true;
        cookingObjects[0].SetActive(true);
    }

    public void StartCooking()
    {
        isCooking = true;
        hasCooked = true;
        cookingObjects[0].SetActive(false);
    }

    public void FinishCooking()
    {
        isCooking = false;
    }

    public void FinishRequest()
    {
        hasRequest = false;
        hasCooked = false;
        StartCoroutine(DespawnKit());
    }

    public void SetIndexCooking(CookStage stage)
    {
        cookingIndex = (int)stage;
        cookingObjects[cookingIndex].SetActive(true);
        Debug.Log($"[CookingManager] Cooking Index : {cookingIndex}");
    }

    private void SetRequest()
    {
        kitObject.SetActive(true);

        cookingIndex = 0;
        requestIndex = Random.Range(1, 4);
        requestObjects[requestIndex].SetActive(true);
        Debug.Log($"[CookingManager] Request Index : {requestIndex}");
    }

    private void ResetRequest()
    {
        kitObject.SetActive(false);

        if (requestIndex == cookingIndex)
        {
            timerManager.AddTime(10);
            scoreManager.AddScore(20);
            scoreManager.AddCarrot(10);
        }
        else
        {
            timerManager.AddTime(-5);
            scoreManager.AddScore(-10);
            scoreManager.AddCarrot(-5);
        }

        Debug.Log($"[CookingManager] Reset Request");
        requestObjects[requestIndex].SetActive(false);
        cookingObjects[cookingIndex].SetActive(false);
        requestIndex = -1;
        cookingIndex = -1;
    }

    public int G_IndexRequest => requestIndex;
    public int G_IndexCooking => cookingIndex;
    public bool G_HasRequest => hasRequest;
    public bool G_HasCooked => hasCooked;
    public bool G_IsCooking => isCooking;
}