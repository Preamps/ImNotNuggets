using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    
    public int totalWaves = 5;
    public float timeBetweenWaves = 3f;
    public int startEnemyCount = 3;  
    public float enemyIncreasePerWave = 1.5f;

    private int currentWave = 0;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    private bool waveActive = false;

    private void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private void Update()
    {
        // check if all enemies died then start next wave
        if (waveActive && aliveEnemies.Count == 0)
        {
            waveActive = false;
            StartCoroutine(StartNextWave());
        }
    }

    IEnumerator StartNextWave()
    {
        if (currentWave >= totalWaves)
        {
            Debug.Log("ALL WAVES COMPLETED!");
            yield break;
        }

        currentWave++;
        Debug.Log("Wave " + currentWave + " is starting...");

        yield return new WaitForSeconds(timeBetweenWaves);

        int enemyCount = Mathf.RoundToInt(startEnemyCount * Mathf.Pow(enemyIncreasePerWave, currentWave - 1));

        SpawnWave(enemyCount);
    }

    void SpawnWave(int count)
    {
        aliveEnemies.Clear();
        waveActive = true;

        for (int i = 0; i < count; i++)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawn.position, Quaternion.identity);
            aliveEnemies.Add(enemy);

            // When enemy dies will remove from list
            EnemyDeathNotify deathNotify = enemy.AddComponent<EnemyDeathNotify>();
            deathNotify.manager = this;
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        aliveEnemies.Remove(enemy);
    }
}
