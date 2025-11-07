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

    
    void Start()
    {
        Init(100);
        //healthBar.SetMaxHealth(Health);
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position +  movement * moveSpeed);
    }
}
