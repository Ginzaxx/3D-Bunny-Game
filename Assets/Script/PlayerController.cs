using UnityEngine;

/// <summary>
/// PlayerController - Mengatur gerakan Kelinci kiri/kanan dan animasi
/// Attach ke GameObject Kelinci
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float boundaryLeft = -8f;
    public float boundaryRight = 8f;

    [Header("Catch Settings")]
    public float catchWidth = 1.5f;   // lebar keranjang / area tangkap
    public Transform basketTransform;  // keranjang kelinci

    [Header("Visual Feedback")]
    public Animator animator;
    public ParticleSystem catchEffect;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private WeatherManager weatherManager;
    private int score = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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

        // Keyboard input
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            input = -1f;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            input = 1f;

        // Mobile touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float screenCenter = Screen.width / 2f;
            input = (touch.position.x < screenCenter) ? -1f : 1f;
        }

        float speed = moveSpeed;
        // Salju: kecepatan berkurang
        if (weatherManager != null && weatherManager.CurrentWeather == WeatherType.Snow)
            speed *= 0.6f;

        targetPosition = transform.position + Vector3.right * input * speed * Time.deltaTime;

        // Animasi jalan
        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(input));

        // Flip sprite / rotation berdasar arah
        if (input != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (input > 0 ? 1 : -1);
            transform.localScale = scale;
        }
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
        targetPosition.x = Mathf.Clamp(targetPosition.x, boundaryLeft, boundaryRight);
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
            OnCatchSuccess(obj);
            return true;
        }
        return false;
    }

    void OnCatchSuccess(FallingObject obj)
    {
        score++;
        if (catchEffect != null)
            catchEffect.Play();

        if (animator != null)
            animator.SetTrigger("Catch");
    }

    public int Score => score;
}
