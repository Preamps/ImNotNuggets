using UnityEngine;

public class EnemyDeathNotify : MonoBehaviour
{
    public WaveManager manager;

    public void OnDeath()
    {
        manager.RemoveEnemy(gameObject);
        Destroy(gameObject);
    }
}
