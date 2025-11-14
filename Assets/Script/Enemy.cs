using UnityEngine;

public class Enemy : Character
{
    private int damageHit;
    public Transform player;
    public float speed = 2f;
    public int DamageHit
    {
        get
        {
            return damageHit;
        }
        set
        {
            damageHit = value;
        }
    }
    private void Update()
    {
        Enemymove();
    }

    public void Enemymove() // enemy เดินหาผู้เล่น
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        
        transform.position = (Vector2)transform.position +
                            direction * speed * Time.deltaTime;

    }

}
