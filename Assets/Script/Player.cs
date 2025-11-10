using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public HealthBar healthBar;
    public float moveSpeed = 0.2f;
    Vector2 movement;

    public void TakeDamage(int damage)
    {
        base.TakeDamage(damage); // ลดเลือดตามระบบ Character
        healthBar.UpdateHealthBar(Health); // แสดง UI เฉพาะ Player
    }

    public SpriteRenderer spriteRenderer; // ใส่ sprite player
    public Sprite rightSprite;
    public Sprite leftSprite;


    void Start()
    {
        Init(100);
        //healthBar.SetMaxHealth(Health);

        // Setup default sprite
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");


        // ทิศทางการหันของตัวละคร
        if (movement.x > 0)
        {
            spriteRenderer.sprite = rightSprite;
        }
        else if (movement.x < 0)
        {
            spriteRenderer.sprite = leftSprite;
        }
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position +  movement * moveSpeed);
    }
}
