using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Player : Character
{
    
    public float moveSpeed = 5f;          // Top speed
    public float acceleration = 10f;      // How fast player accelerates
    public float deceleration = 10f;      // How fast player slows down

    
    
    public SpriteRenderer spriteRenderer;

    
    public Sprite playerSprite;           

    private Vector2 movement;
    private Vector2 currentVelocity;      // Current velocity (for inertia)
    private Vector2 velocitySmoothing;    // SmoothDamp helper

    private Rigidbody2D rb;

    public int maxAmmo = 12;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;

    public TMP_Text ammoText;
    public Image reloadCircle;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (playerSprite != null)
            spriteRenderer.sprite = playerSprite;

        Init(100);  // From Character

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
            spriteRenderer.flipX = false;   // face right
        else if (currentVelocity.x < -0.1f)
            spriteRenderer.flipX = true;    // face left

        if (currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }

        // --- Shooting ---
        if (Input.GetMouseButtonDown(0) && !isReloading)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    void FixedUpdate()
    {
        // --- Calculate target velocity ---
        Vector2 targetVelocity = movement * moveSpeed;

        // --- Smooth acceleration / deceleration ---
        currentVelocity = Vector2.SmoothDamp(
            currentVelocity,
            targetVelocity,
            ref velocitySmoothing,
            movement.magnitude > 0 ? 1f / acceleration : 1f / deceleration
        );

        // --- Move player ---
        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }

    // --- Damage handling ---
    //public void TakeDamage(int damage)
    //{
       // base.TakeDamage(damage);             // Character reduces heal
    //}
    

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    void Shoot()
    {

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        currentAmmo--;
        UpdateAmmoUI();

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)firePoint.position).normalized;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Init(direction,bulletSpeed); // ส่งทิศทางยิง
        }
    }
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        // Hide ammo text while reloading
        if (ammoText != null)
            ammoText.enabled = false;

        if (reloadCircle != null)
            reloadCircle.fillAmount = 0f;

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

        // Show ammo text again after reload
        if (ammoText != null)
            ammoText.enabled = true;

        UpdateAmmoUI();

        if (reloadCircle != null)
            reloadCircle.fillAmount = 0f; // hide circle after reload

        Debug.Log("Reloaded!");
    }


    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }

    // UI Ammo
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;

}
