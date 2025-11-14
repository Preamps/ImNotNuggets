using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 direction;

    public void Init(Vector2 shootDir, float bulletSpeed)
    {
        direction = shootDir.normalized;
        speed = bulletSpeed;
    }

    private void Start()
    {
        Destroy(gameObject, 5f); // หายเอง 5 วิ
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)(direction * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(10);
            Destroy(gameObject);
        }
    }
}
