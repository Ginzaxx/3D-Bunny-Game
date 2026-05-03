using UnityEngine;

public enum FoodList
{
    Raw,
    Grilled,
    Chopped,
    ChoppedGrilled,
    Boiled,
    BoiledChopped,
    BoiledGrilled,
    FullyCooked,
}

public class CookingManager : MonoBehaviour
{
    public static CookingManager Instance { get; private set; }

    [Header("Cooking Prefab List")]
    [SerializeField] private GameObject RawPrefab;
    [SerializeField] private GameObject ChoppedPrefab;
    [SerializeField] private GameObject BoiledPrefab;
    [SerializeField] private GameObject GrilledPrefab;
    [SerializeField] private GameObject ChoppedBoiledPrefab;
    [SerializeField] private GameObject ChoppedGrilledPrefab;
    [SerializeField] private GameObject BoiledGrilledPrefab;
    [SerializeField] private GameObject FullyCookedPrefab;

    [Header("Food Variables")]
    [SerializeField] private int requestIndex;
    [SerializeField] private bool hasRequest;
    [SerializeField] private bool isCooking;
    [SerializeField] private bool chopped = false;
    [SerializeField] private bool boiled = false;
    [SerializeField] private bool grilled = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void UpdateCooking()
    {
        if (chopped)
        {
            ;
        }
        else
        {
            ;
        }
    }

    public void SetHasRequest(bool enabled) 
    {
        hasRequest = enabled;
    }

    public void SetIsCooking(bool enabled, string method)
    {
        isCooking = enabled;

        if (enabled)
        {
            switch (method)
            {
            case "Board":
                chopped = true;
                break;
            case "Pot":
                boiled = true;
                break;
            case "Grill":
                grilled = true;
                break;
            }
        }

        UpdateCooking();
    }

    public bool GetHasRequest => hasRequest;
    public bool GetIsCooking => isCooking;
}
