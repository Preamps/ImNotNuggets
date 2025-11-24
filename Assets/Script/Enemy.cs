using UnityEngine;

public class Enemy : Character
{
    public int damageHit = 10;
    private Transform player;
    public float speed = 2f;

    public float avoidRadius = 0.8f;
    public float avoidStrength = 2f;

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

    private void Start()
    {
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    
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

        SeparateFromOthers();

    }

    void SeparateFromOthers()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, avoidRadius);

        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject && hit.CompareTag("Enemy"))
            {
                // push away
                Vector2 push = (Vector2)(transform.position - hit.transform.position).normalized;
                transform.position += (Vector3)(push * avoidStrength * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Character player = collision.GetComponent<Character>();
            if (player != null)
            {
                player.TakeDamage(damageHit);
            }
        }
    }


}
