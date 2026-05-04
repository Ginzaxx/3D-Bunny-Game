using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds1 = new(1);

    [Header("References")]
    [SerializeField] private GameObject interactIcon;
    [SerializeField] private CookingManager cooking;

    [Header("Cooking Parameters")]
    [SerializeField] private int cookingTimer = 5;
    [SerializeField] private int cookingCounter;
    [SerializeField] private bool finishCooking;

    public void OnInteract()
    {
        Debug.Log("Interacting with " + tag);

        switch(tag)
        {
        case "Board":
        case "Grill":
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (interactIcon != null && interactIcon.activeSelf)
            interactIcon.SetActive(false);
    }

    private void HandleCooking()
    {
        if (cooking == null) return;
        if (!cooking.G_HasRequest) return;

        if (!cooking.G_IsCooking)
        {
            if (CompareTag("Board") && cooking.IsChopped)
            {
                Debug.Log("[Interactable] Food is already Chopped");
                return;
            }

            if (CompareTag("Board") && cooking.IsGrilled)
            {
                Debug.Log("[Interactable] Food is already Grilled");
                return;
            }

            if (CompareTag("Board") && cooking.IsBoiled)
            {
                Debug.Log("[Interactable] Food is already Boiled");
                return;
            }

            Debug.Log("[Interactable] Started Cooking");

            cooking.StartCooking();
            StartCoroutine(CookingRoutine());
        }
        else if (finishCooking)
        {
            Debug.Log("[Interactable] Taken Cooking");

            cooking.TakeCooking();
            finishCooking = false;
        }
    }

    IEnumerator CookingRoutine()
    {
        while (cookingCounter < cookingTimer)
        {
            yield return _waitForSeconds1;
            cookingCounter += 1;
        }

        cooking.SetIndexCooking(tag);
        finishCooking = true;

        Debug.Log("[Interactable] Finished Cooking with new Cooking Index " + cooking.G_IndexCooking);
    }

    private void HandleRequest()
    {
        if (cooking == null) return;

        if (!cooking.G_HasRequest)
        {
            Debug.Log("[Interactable] Taken Request");
            cooking.TakeRequest();
        }
        else
        {
            Debug.Log("[Interactable] Finished Request");
            cooking.FinishRequest();
        }
    }
}
