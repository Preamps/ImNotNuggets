using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    
    public float moveSpeed = 5f;          // Top speed
    public float acceleration = 10f;      // How fast player accelerates
    public float deceleration = 10f;      // How fast player slows down

    
    public HealthBar healthBar;
    public SpriteRenderer spriteRenderer;

    
    public Sprite playerSprite;           // Only 1 sprite needed, we will flipX

    private Vector2 movement;
    private Vector2 currentVelocity;      // Current velocity (for inertia)
    private Vector2 velocitySmoothing;    // SmoothDamp helper

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (playerSprite != null)
            spriteRenderer.sprite = playerSprite;

        Init(100);  // From Character
        if (healthBar != null)
            healthBar.UpdateHealthBar(Health);
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
    public void TakeDamage(int damage)
    {
        base.TakeDamage(damage);             // Character reduces health
        if (healthBar != null)
            healthBar.UpdateHealthBar(Health);
    }
}
