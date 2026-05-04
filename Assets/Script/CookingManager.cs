using UnityEngine;

public class CookingManager : MonoBehaviour
{
    [Header("Cooking Prefab List")]
    [SerializeField] private GameObject CookingObject;
    [SerializeField] private GameObject RawPrefab;              // Index 1
    [SerializeField] private GameObject ChoppedPrefab;          // Index 2
    [SerializeField] private GameObject GrilledPrefab;          // Index 3
    [SerializeField] private GameObject ChoppedGrilledPrefab;   // Index 4
    [SerializeField] private GameObject BoiledPrefab;           // Index 5
    [SerializeField] private GameObject ChoppedBoiledPrefab;    // Index 6
    [SerializeField] private GameObject GrilledBoiledPrefab;    // Index 7
    [SerializeField] private GameObject FullyCookedPrefab;      // Index 8

    [Header("Food Variables")]
    [SerializeField] private int indexRequest;
    [SerializeField] private int indexCooking;
    [SerializeField] private bool hasRequest;
    [SerializeField] private bool isCooking;
    [SerializeField] private bool boiled = false;
    [SerializeField] private bool chopped = false;
    [SerializeField] private bool grilled = false;

    [Header("References")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameObject kitObject;

    private void UpdateCooking()
    {
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
                CookingObject = ChoppedGrilledPrefab;
                break;
            case 5:
                CookingObject = BoiledPrefab;
                break;
            case 6:
                CookingObject = ChoppedBoiledPrefab;
                break;
            case 7:
                CookingObject = GrilledBoiledPrefab;
                break;
            case 8:
                CookingObject = FullyCookedPrefab;
                break;
        }

        Debug.Log("[CookingManager] New Cooking Index : " + indexCooking);
    }

    public void SetIndexCooking(string method)
    {
        switch (method)
        {
        case "Board":
            chopped = true;
            indexCooking += 1;
            break;
        case "Grill":
            grilled = true;
            indexCooking += 2;
            break;
        case "Pot":
            boiled = true;
            indexCooking += 4;
            break;
        }

        UpdateCooking();
    }

    public void StartCooking()
    {
        isCooking = true;
    }

    public void TakeCooking()
    {
        isCooking = false;
    }

    public void SetRequest()
    {
        if (kitObject != null && !kitObject.activeSelf)
            kitObject.SetActive(true);

        indexRequest = Random.Range(1, 8);
        indexCooking = 1;

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
        
        hasRequest = false;
        isCooking = false;

        indexRequest = 0;
        indexCooking = 0;

        chopped = false;
        grilled = false;
        boiled = false;

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
    public bool IsChopped => chopped;
    public bool IsGrilled => grilled;
    public bool IsBoiled => boiled;
}
