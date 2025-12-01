using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Rigidbody2D targetRb;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    // --- Shake ---
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.3f;
    private Vector3 initialOffset;

    void Start()
    {
        initialOffset = offset;
    }

    void FixedUpdate()
    {
        if (targetRb == null) return;

        Vector3 targetPos = targetRb.position + new Vector2(offset.x, offset.y);
        targetPos.z = offset.z;

        // --- Apply Shake ---
        if (shakeDuration > 0)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
            targetPos += new Vector3(offsetX, offsetY, 0f);

            shakeDuration -= Time.fixedDeltaTime;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.fixedDeltaTime);
    }

    // --- Public method to trigger shake ---
    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
