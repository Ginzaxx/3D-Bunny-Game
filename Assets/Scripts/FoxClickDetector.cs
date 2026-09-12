using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// FoxClickDetector - Menampilkan countdown UI di atas Rubah
/// dan deteksi click/tap mobile
/// Attach ke prefab Rubah bersama FallingObject
/// </summary>
public class FoxClickDetector : MonoBehaviour
{
    [Header("UI Countdown")]
    public GameObject countdownCanvas;   // World Space Canvas
    public Image timerFillImage;         // Fill image countdown lingkaran
    public TextMeshProUGUI countdownText;

    [Header("Settings")]
    public float clickWindow = 2f;

    [Header("Visual")]
    public SpriteRenderer foxSprite;
    public Animator foxAnimator;

    private float timer;
    private bool isActive = true;
    private FallingObject fallingObject;
    private Camera mainCamera;

    void Start()
    {
        fallingObject = GetComponent<FallingObject>();
        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        timer = clickWindow;
        isActive = true;
        
        if (countdownCanvas != null) countdownCanvas.SetActive(true);
        if (foxAnimator != null)
        {
            foxAnimator.SetTrigger("Appear");
            // Reset other triggers if necessary
            foxAnimator.ResetTrigger("Caught");
        }

        // Mulai animasi muncul
        StartCoroutine(AppearAnimation());
    }

    void Update()
    {
        if (!isActive) return;

        timer -= Time.deltaTime;

        // Update UI countdown
        if (timerFillImage != null)
            timerFillImage.fillAmount = timer / clickWindow;

        if (countdownText != null)
            countdownText.text = Mathf.CeilToInt(timer).ToString();

        // Warna berubah saat mendekati habis
        if (timerFillImage != null)
        {
            float t = timer / clickWindow;
            timerFillImage.color = Color.Lerp(Color.red, Color.green, t);
        }

        // Deteksi touch/click
        if (Input.GetMouseButtonDown(0))
        {
            CheckClick(Input.mousePosition);
        }

        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
                CheckClick(Input.GetTouch(i).position);
        }
    }

    void CheckClick(Vector2 screenPos)
    {
        if (!isActive) return;

        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.gameObject == gameObject)
            {
                OnFoxClicked();
            }
        }
    }

    void OnFoxClicked()
    {
        isActive = false;
        fallingObject?.FoxClicked();

        // Efek click berhasil
        if (foxAnimator != null) foxAnimator.SetTrigger("Caught");
        if (countdownCanvas != null) countdownCanvas.SetActive(false);

        StartCoroutine(DestroyAfterAnimation());
    }

    IEnumerator AppearAnimation()
    {
        // Scale in animation
        transform.localScale = Vector3.zero;
        float t = 0;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / 0.3f);
            yield return null;
        }
        transform.localScale = Vector3.one;
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
