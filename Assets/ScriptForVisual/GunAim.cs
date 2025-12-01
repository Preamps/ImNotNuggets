using UnityEngine;

public class GunAim : MonoBehaviour
{
    private Camera cam;
    private SpriteRenderer sr;

    // --- Recoil settings ---
    public float recoilDistance = 0.15f;   // how far gun moves back
    public float recoilSpeed = 10f;        // how fast it returns
    private Vector3 originalLocalPos;      // starting local pos
    private float recoilAmount = 0f;       // current recoil

    void Start()
    {
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();

        // save the gun/hand's local starting position
        originalLocalPos = transform.localPosition;
    }

    void Update()
    {
        AimAtMouse();
        HandleRecoil();
    }

    void AimAtMouse()
    {
        // Convert mouse to world position
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Direction from gun → mouse
        Vector2 dir = (mousePos - transform.position).normalized;

        // Calculate rotation angle
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Flip sprite when aiming left
        if (angle > 90 || angle < -90)
            sr.flipY = true;
        else
            sr.flipY = false;
    }

    void HandleRecoil()
    {
        // Move recoilAmount back toward 0 smoothly
        recoilAmount = Mathf.Lerp(recoilAmount, 0f, Time.deltaTime * recoilSpeed);

        // Apply recoil backwards relative to gun's rotation
        transform.localPosition = originalLocalPos - transform.right * recoilAmount;
    }

    // Call this when shooting
    public void AddRecoil()
    {
        recoilAmount = recoilDistance;
    }
}
