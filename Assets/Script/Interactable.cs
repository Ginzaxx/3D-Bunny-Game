using System.Collections;
using TMPro;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds1 = new(1);

    [Header("References")]
    [SerializeField] private GameObject interactIcon;
    [SerializeField] private GameObject timerIcon;
    [SerializeField] private GameObject timerNeedle;
    [SerializeField] private CookingManager cooking;
    [SerializeField] private CookStage stage;

    [Header("Cooking Parameters")]
    [SerializeField] private float cookingTimer = 5;
    [SerializeField] private float cookingCounter;
    [SerializeField] private bool finishCooking;

    void Start()
    {
        if (interactIcon != null && interactIcon.activeSelf)
            interactIcon.SetActive(false);
        if (timerIcon != null && timerIcon.activeSelf)
            timerIcon.SetActive(false);

        cooking = FindObjectOfType<CookingManager>();
    }

    public void OnInteract()
    {
        Debug.Log($"[Interactable] Interacting with {stage} Object");

        switch(stage)
        {
        case CookStage.Request:
            HandleRequest();
            break;
        case CookStage.Chopped:
        case CookStage.Grilled:
        case CookStage.Boiled:
            HandleCooking();
            break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (interactIcon == null || interactIcon.activeSelf) return;
        if (stage != CookStage.Request && (cooking.G_IsCooking || !cooking.G_HasRequest)) return;
        interactIcon.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (interactIcon == null || !interactIcon.activeSelf) return;
        if (stage != CookStage.Request && (cooking.G_IsCooking || !cooking.G_HasRequest)) return;
        interactIcon.SetActive(false);
    }

    private void HandleRequest()
    {
        if (cooking == null) return;

        if (!cooking.G_HasRequest)
        {
            Debug.Log("[Interactable] Started Request");
            cooking.StartRequest();
            PlayCookingSFX();
        }
        else if (!cooking.G_IsCooking)
        {
            Debug.Log("[Interactable] Finished Request");
            cooking.FinishRequest();
        }
        else
        {
            Debug.Log("[Interactable] Unable to Finish Request");
        }
    }

    private void HandleCooking()
    {
        if (cooking == null) return;
        if (!cooking.G_HasRequest) return;

        if (!cooking.G_HasCooked)
        {
            Debug.Log("[Interactable] Started Cooking");
            cooking.StartCooking();
            PlayCookingSFX();
            StartCoroutine(CookingRoutine());
        }
        else if (finishCooking)
        {
            Debug.Log("[Interactable] Finished Cooking");
            cooking.FinishCooking();
            cooking.SetIndexCooking(stage);
            PlayTimerSFX();
            finishCooking = false;
        }
        else
        {
            Debug.Log("[Interactable] Unable to Cook");
        }
    }

    IEnumerator CookingRoutine()
    {
        timerIcon.SetActive(true);
        interactIcon.SetActive(false);
        cookingCounter = cookingTimer;

        while (cookingCounter > 0)
        {
            cookingCounter -= Time.deltaTime;
            timerNeedle.transform.rotation = Quaternion.Euler(270 - (cookingCounter * 72), 90, -90);
            yield return Time.deltaTime;
        }

        finishCooking = true;
        timerIcon.SetActive(false);
        interactIcon.SetActive(true);
        Debug.Log("[Interactable] Finished Cooking with New Cooking Index " + cooking.G_IndexCooking);
    }

    void PlayCookingSFX()
    {
        switch (stage)
        {
        case CookStage.Request:
            AudioManager.Instance.PlayKit();
            break;
        case CookStage.Chopped:
            AudioManager.Instance.PlayChopping();
            break;
        case CookStage.Grilled:
            AudioManager.Instance.PlayGrilling();
            break;
        case CookStage.Boiled:
            AudioManager.Instance.PlayBoiling();
            break;
        }
    }

    void PlayTimerSFX()
    {
        AudioManager.Instance.PlayTimer();
    }
}