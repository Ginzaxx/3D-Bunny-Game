using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds1 = new(1);

    [Header("References")]
    [SerializeField] private GameObject interactIcon;

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
        case "Pot":
        case "Grill":
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
        if (CookingManager.Instance == null) return;
        if (!CookingManager.Instance.GetHasRequest) return;

        if (!CookingManager.Instance.GetIsCooking)
        {
            Debug.Log("Started Cooking");

            CookingManager.Instance.SetIsCooking(true, tag);
            StartCoroutine(CookingRoutine());
        }
        else if (finishCooking)
        {
            Debug.Log("Taken Cooking");

            CookingManager.Instance.SetIsCooking(false, tag);
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

        Debug.Log("Finished Cooking");
        finishCooking = true;
    }

    private void HandleRequest()
    {
        if (CookingManager.Instance == null) return;

        if (!CookingManager.Instance.GetHasRequest)
        {
            Debug.Log("Taken Request");
            CookingManager.Instance.SetHasRequest(true);
        }
        else
        {
            Debug.Log("Given Cooking");
            CookingManager.Instance.SetHasRequest(false);
        }
    }
}
