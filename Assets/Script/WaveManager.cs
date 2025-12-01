using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WaveManager : MonoBehaviour
{
    
    public GameObject[] enemyPrefabs;   // list of enemy types

    public Transform[] spawnPoints;

    public WavesCompleteUIManager wavesUI;

    public TMP_Text waveText;


    public int totalWaves = 5;
    public float timeBetweenWaves = 3f;
    public int startEnemyCount = 3;
    public float enemyIncreasePerWave = 1.5f;

    private int currentWave = 0;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    private bool waveActive = false;

    private void Start()
    {
        // ตรวจว่าเป็น Start ใหม่ หรือ Restart
        bool isNewGame = PlayerPrefs.GetInt("StartNewGame", 0) == 1;
       
        if (isNewGame)
        {
            // เริ่มใหม่ → เริ่ม wave 1
            currentWave = 0;
            WaveSaveManager.ResetWave();
            PlayerPrefs.SetInt("StartNewGame", 0); // ล้าง flag
        }
        else
        {
            // Restart → โหลด wave ล่าสุด
            int savedWave = WaveSaveManager.LoadWave(); // เช่น 5
            currentWave = savedWave - 1; // -1 เพราะ wave จะถูก ++ ใน StartNextWave()
        }

        StartCoroutine(StartNextWave());
    }

    private void Update()
    {
        if (waveActive && aliveEnemies.Count == 0)
        {
            waveActive = false;
            StartCoroutine(StartNextWave());
            SoundManager.Instance.PlaySFX("WaveStart");
        }
    }

    IEnumerator StartNextWave()
    {
        if (currentWave >= totalWaves)
        {
            Debug.Log("ALL WAVES COMPLETED!");

            if (wavesUI != null)
                wavesUI.ShowWavesCompleteUI();
            if (waveText != null)
                waveText.text = "All Waves Completed!";

            SoundManager.Instance.PlaySFX("Win");
            yield break;
        }

        currentWave++;
        WaveSaveManager.SaveWave(currentWave);

        Debug.Log("Wave " + currentWave + " is starting...");

        if (waveText != null)
        {
            waveText.text = currentWave + " / " + totalWaves;
        }

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

            // Pick a random enemy prefab
            GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            GameObject enemyObj = Instantiate(randomEnemy, spawn.position, Quaternion.identity);
            aliveEnemies.Add(enemyObj);

            // Add death notifier
            EnemyDeath notifier = enemyObj.AddComponent<EnemyDeath>();
            notifier.manager = this;
            notifier.enemyObject = enemyObj;
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        aliveEnemies.Remove(enemy);
    }

 
    public class EnemyDeath : MonoBehaviour
    {
        public WaveManager manager;
        public GameObject enemyObject;

        public void OnDeath()
        {
            manager.RemoveEnemy(enemyObject);
            Destroy(enemyObject);
        }
    }
}
