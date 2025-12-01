using UnityEngine;

public class SmoothMenuCameraWithZone : MonoBehaviour
{
    [Header("Settings")]
    public float maxMoveX = 5f;       // Max horizontal movement
    public float maxMoveY = 3f;       // Max vertical movement
    public float smoothSpeed = 5f;    // How smooth it moves

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Get mouse position in normalized screen coordinates (-1 to 1)
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Calculate target position and clamp it within max zone
        float targetX = Mathf.Clamp(mouseX * maxMoveX, -maxMoveX, maxMoveX);
        float targetY = Mathf.Clamp(mouseY * maxMoveY, -maxMoveY, maxMoveY);
        Vector3 targetPosition = startPosition + new Vector3(targetX, targetY, 0f);

        // Smoothly move camera/panel
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }
}

