using UnityEngine;

public class HeartBeat : MonoBehaviour
{
    [SerializeField] private RectTransform heartRect;
    [SerializeField] private float beatSpeed = 2f;      // How fast the heart beats
    [SerializeField] private float beatAmount = 0.1f;   // How much it scales

    private Vector3 originalScale;

    private void Awake()
    {
        if (heartRect == null)
            heartRect = GetComponent<RectTransform>();

        originalScale = heartRect.localScale;
    }

    private void Update()
    {
        // Ping-pong scale using sine wave
        float scale = 1 + Mathf.Sin(Time.time * beatSpeed) * beatAmount;
        heartRect.localScale = originalScale * scale;
    }
}
