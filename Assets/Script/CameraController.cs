using UnityEngine;

/// <summary>
/// CameraController - Kamera 3D dengan sudut miring ke atas
/// Sesuai desain: "kamera dibuat agak miring ke atas"
/// Attach ke Main Camera
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Camera Position")]
    public Vector3 gameplayOffset = new Vector3(0f, 8f, -6f);
    public Vector3 gameplayRotation = new Vector3(45f, 0f, 0f); // miring ke atas

    [Header("Follow Target (opsional)")]
    public Transform target;          // set ke player jika mau follow
    public bool followPlayerX = false; // false = kamera statis
    public float followSmoothing = 5f;

    [Header("Menu Camera")]
    public Vector3 menuOffset = new Vector3(0f, 5f, -10f);
    public Vector3 menuRotation = new Vector3(20f, 0f, 0f);

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        SetGameplayCamera();
    }

    void LateUpdate()
    {
        if (followPlayerX && target != null)
        {
            // Smooth follow player di sumbu X saja
            Vector3 desiredPos = target.position + gameplayOffset;
            desiredPos.x = Mathf.Lerp(transform.position.x, target.position.x, Time.deltaTime * followSmoothing) + gameplayOffset.x;
            desiredPos.y = gameplayOffset.y;
            desiredPos.z = gameplayOffset.z;
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSmoothing);
        }
        else
        {
            // Kamera statis, smooth ke target
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 8f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
        }
    }

    public void SetGameplayCamera()
    {
        targetPosition = gameplayOffset;
        targetRotation = Quaternion.Euler(gameplayRotation);
    }

    public void SetMenuCamera()
    {
        targetPosition = menuOffset;
        targetRotation = Quaternion.Euler(menuRotation);
    }

    /// <summary>
    /// Shake kamera saat efek tertentu (misal rubah berhasil klik)
    /// </summary>
    public void ShakeCamera(float intensity = 0.2f, float duration = 0.3f)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    System.Collections.IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            transform.localPosition = originalPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
