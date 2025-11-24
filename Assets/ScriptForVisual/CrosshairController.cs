using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public RectTransform crosshair;

    void Start()
    {
        // Hide the default cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined; // optional, keeps cursor inside window
    }

    void Update()
    {
        // Move crosshair to mouse position
        Vector2 mousePos = Input.mousePosition;
        crosshair.position = mousePos;
    }
}
