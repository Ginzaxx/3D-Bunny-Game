using UnityEngine;

/// <summary>
/// PlayerController - Mengatur gerakan Kelinci kiri/kanan dan animasi
/// Attach ke GameObject Kelinci
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float boundaryLeft  = -8f;
    public float boundaryRight =  8f;

    [Header("Catch Settings")]
    public float catchWidth = 1.5f;
    public Transform basketTransform;

    [Header("Visual Feedback")]
    public Animator animator;
    public ParticleSystem catchEffect;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private WeatherManager weatherManager;

    void Start()
    {
        rb             = GetComponent<Rigidbody>();
        weatherManager = FindObjectOfType<WeatherManager>();
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleInput();
        ClampPosition();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void HandleInput()
    {
        float input = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            input = -1f;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            input = 1f;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float screenCenter = Screen.width / 2f;
            input = (touch.position.x < screenCenter) ? -1f : 1f;
        }

        float speed = moveSpeed;
        if (weatherManager != null && weatherManager.CurrentWeather == WeatherType.Snow)
            speed *= 0.6f;

        targetPosition = transform.position + Vector3.right * input * speed * Time.deltaTime;

        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(input));
    }

    void MovePlayer()
    {
        rb.MovePosition(targetPosition);
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, boundaryLeft, boundaryRight);
        transform.position = pos;
        targetPosition.x   = Mathf.Clamp(targetPosition.x, boundaryLeft, boundaryRight);
    }

    /// <summary>
    /// Dipanggil dari FallingObject saat objek menyentuh keranjang
    /// </summary>
    public bool TryCatch(FallingObject obj)
    {
        if (basketTransform == null) return false;

        float dist = Mathf.Abs(obj.transform.position.x - basketTransform.position.x);
        if (dist <= catchWidth)
        {
            OnCatchSuccess();
            return true;
        }
        return false;
    }

    void OnCatchSuccess()
    {
        // Score & carrot sekarang ditangani sepenuhnya di FallingObject.OnCaught()
        // yang memanggil ScoreManager.AddCarrot() dan ScoreManager.AddScore()
        if (catchEffect != null)
            catchEffect.Play();

        if (animator != null)
            animator.SetTrigger("Catch");
    }
}