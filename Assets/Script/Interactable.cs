using System.Collections;
using TMPro;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds1 = new(1);

    [Header("References")]
    [SerializeField] private GameObject interactIcon;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private CookingManager cooking;

    [Header("Cooking Parameters")]
    [SerializeField] private int cookingTimer = 5;
    [SerializeField] private int cookingCounter;
    [SerializeField] private bool finishCooking;

    void Start()
    {
        cooking = FindObjectOfType<CookingManager>();
    }

    public void OnInteract()
    {
        Debug.Log("[Interactable] Interacting with " + tag);

        switch(tag)
        {
        case "Board":
            HandleCooking();
            break;
        case "Grill":
            HandleCooking();
            break;
        case "Pot":
            HandleCooking();
            break;
        case "Kit":
            HandleRequest();
            break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (interactIcon != null && !interactIcon.activeSelf)
            interactIcon.SetActive(true);

        if (timerText != null && !timerText.gameObject.activeSelf)
            timerText.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (interactIcon != null && interactIcon.activeSelf)
            interactIcon.SetActive(false);

        if (timerText != null && timerText.gameObject.activeSelf)
            timerText.gameObject.SetActive(false);
    }

    private void HandleCooking()
    {
        if (cooking == null) return;
        if (!cooking.G_HasRequest) return;

        if (!cooking.G_IsCooking || !cooking.G_IsCooked)
        {
            Debug.Log("[Interactable] Started Cooking");

            cooking.StartCooking();
            StartCoroutine(CookingRoutine());
        }
        else if (finishCooking)
        {
            Debug.Log("[Interactable] Finished Cooking");

            cooking.FinishCooking();
            finishCooking = false;
        }
        else
        {
            Debug.Log("[Interactable] Unable to Perform Cooking");
        }
    }

    IEnumerator CookingRoutine()
    {
        cookingCounter = cookingTimer;

        while (cookingCounter > 0)
        {
            yield return _waitForSeconds1;
            cookingCounter -= 1;
            timerText.text = $"{cookingCounter}";
        }

        finishCooking = true;
        timerText.text = "";
        cooking.SetIndexCooking(tag);

        Debug.Log("[Interactable] Finished Cooking with New Cooking Index " + cooking.G_IndexCooking);
    }

    private void HandleRequest()
    {
        if (cooking == null) return;

        if (!cooking.G_HasRequest)
        {
            Debug.Log("[Interactable] Taken Request");
            cooking.StartRequest();
        }
        else
        {
            Debug.Log("[Interactable] Finished Request");
            cooking.FinishRequest();
        }
    }
}
