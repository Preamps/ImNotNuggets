using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : Character
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 10f;

    [Header("Camera")]
    public Camera mainCamera;  // assign in inspector

    [Header("Sprite")]
    public SpriteRenderer spriteRenderer;
    public Sprite playerSprite;

    [Header("Ammo")]
    public int maxAmmo = 12;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;
    public TMP_Text ammoText;
    public Image reloadCircle;

    [Header("Gun")]
    public GunAim gunAim;
    public MuzzleFlash muzzleFlash;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    // Private movement helpers
    private Vector2 movement;
    private Vector2 currentVelocity;
    private Vector2 velocitySmoothing;

    public WaveManager waveManager;
    private bool _alreadyDead = false;

    void Awake()
    {
        // ถ้าใน Inspector ลืมลากใส่ ให้มันหาเองในฉาก
        if (waveManager == null)
        {
            waveManager = GameObject.FindObjectOfType<WaveManager>();
        }
    }

    void Start()
    {
        if (spriteRenderer == null)
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (playerSprite != null)
            spriteRenderer.sprite = playerSprite;

        Init(100); // From Character

        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        if (reloadCircle != null)
            reloadCircle.fillAmount = 0f; // hide reload circle
    }

    void Update()
    {
        // --- Input ---
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        // --- Sprite direction ---
        if (currentVelocity.x > 0.1f)
            spriteRenderer.flipX = false;
        else if (currentVelocity.x < -0.1f)
            spriteRenderer.flipX = true;

        // --- Auto reload ---
        if (currentAmmo <= 0 && !isReloading)
            StartCoroutine(Reload());

        // --- Shoot ---
        if (Input.GetMouseButtonDown(0) && !isReloading)
            Shoot();

        // --- Manual reload ---
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
            StartCoroutine(Reload());
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = movement * moveSpeed;

        currentVelocity = Vector2.SmoothDamp(
            currentVelocity,
            targetVelocity,
            ref velocitySmoothing,
            movement.magnitude > 0 ? 1f / acceleration : 1f / deceleration
        );

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }

    IEnumerator ShootFlashYellow()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = Color.white;
        }
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        currentAmmo--;
        UpdateAmmoUI();
        SoundManager.Instance.PlaySFX("GunShot");

        // --- Camera shake ---
        if (mainCamera != null)
        {
            CameraFollow camFollow = mainCamera.GetComponent<CameraFollow>();
            if (camFollow != null)
                camFollow.Shake(0.1f, 1.5f); // duration, magnitude
        }

        // --- Gun effects ---
        if (gunAim != null)
            gunAim.AddRecoil();

        if (muzzleFlash != null)
            muzzleFlash.Flash();

        StartCoroutine(ShootFlashYellow());

        // --- Shoot bullet ---
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)firePoint.position).normalized;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Init(direction, bulletSpeed);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        if (ammoText != null) ammoText.enabled = false;
        if (reloadCircle != null) reloadCircle.fillAmount = 0f;

        SoundManager.Instance.PlaySFX("Reload");

        float elapsed = 0f;
        while (elapsed < reloadTime)
        {
            elapsed += Time.deltaTime;
            if (reloadCircle != null)
                reloadCircle.fillAmount = Mathf.Clamp01(elapsed / reloadTime);
            yield return null;
        }

        currentAmmo = maxAmmo;
        isReloading = false;

        if (ammoText != null) ammoText.enabled = true;
        UpdateAmmoUI();

        if (reloadCircle != null)
            reloadCircle.fillAmount = 0f;
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo}/{maxAmmo}";
    }
    public void Die()
    {
        if (_alreadyDead) return;
        _alreadyDead = true;

        if (waveManager != null)
        {
            // 🔹 สั่งบวกเลขการตายสะสม และส่ง Analytics
            waveManager.RegisterDeath();

            // ย้ายเข้ามาข้างในนี้ เพราะถ้า waveManager เป็น Null บรรทัดนี้จะ Error ทันที
            Debug.Log("<color=red>Player Died!</color> Current Wave: " + waveManager.GetCurrentWave());
        }
        else
        {
            Debug.LogWarning("Player Died! แต่หา WaveManager ไม่เจอ (Null)");
        }
    }


    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
}
