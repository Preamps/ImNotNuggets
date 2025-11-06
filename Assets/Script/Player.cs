using UnityEngine;

public class Player : Character
{
    public HealthBar healthBar;
    public float moveSpeed = 5f;
    private Vector2 moveInput;

    public void TakeDamage(int damage)
    {
        base.TakeDamage(damage); // ลดเลือดตามระบบ Character
        healthBar.UpdateHealthBar(Health); // แสดง UI เฉพาะ Player
    }

    
    void Start()
    {
        Init(100);
        healthBar.SetMaxHealth(Health);
    }

    void Update()
    {
       
    }
   
}
