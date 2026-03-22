using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject[] enemyPrefabs;   // list of enemy type
    public Transform[] spawnPoints;

    [Header("UI")]
    public WavesCompleteUIManager wavesUI;
    public TMP_Text waveText;

    [Header("Wave Setting")]
    public int totalWaves = 5;
    public float timeBetweenWaves = 3f;
    public int startEnemyCount = 3;
    public float enemyIncreasePerWave = 1.5f;

    private int currentWave = 0;
    
    private List<GameObject> aliveEnemies = new List<GameObject>();
    private bool waveActive = false;

    // Analytics
    private int deathCountThisWave = 0;
    private int highestWave = 0;
    private float waveStartTime = 0f;

    private void Start()
    {
        // ตรวจว่าเป็น Start ใหม่ หรือ Restart
        bool isNewGame = PlayerPrefs.GetInt("StartNewGame", 0) == 1;
        
        highestWave = PlayerPrefs.GetInt("HighestWave", 0);

        if (isNewGame)
        {
            // เริ่มใหม่ → เริ่ม wave 1
            currentWave = 0;
            highestWave = 0;
     
            WaveSaveManager.ResetWave();

            PlayerPrefs.SetInt("HighestWave", 0);
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
           
            float timeTaken = Time.time - waveStartTime;
            
            // Time to Complete Rate
            SendWaveAnalytics("time_to_complete_rate", timeTaken);

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

        //  Player Skill Progression
        if (currentWave > highestWave)
        {
            highestWave = currentWave;

            PlayerPrefs.SetInt("HighestWave", highestWave);
            PlayerPrefs.Save();

            SendWaveAnalytics("player_skill_progression", 0f);
        }

        Debug.Log("Wave " + currentWave + " is starting...");

        if (waveText != null)
            waveText.text = currentWave + " / " + totalWaves;

        // Reset Analytics
        deathCountThisWave = 0;
        waveStartTime = Time.time;

        yield return new WaitForSeconds(timeBetweenWaves);

        int enemyCount = Mathf.RoundToInt(
            startEnemyCount * Mathf.Pow(enemyIncreasePerWave, currentWave - 1)
        );

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

    // Player เรียกตอนตาย
    public void RegisterDeath()
    {
        deathCountThisWave++;

        float timeInWave = Time.time - waveStartTime;

        //  Failure Rate (พร้อมเวลา)
        SendWaveAnalytics("failure_rate", timeInWave);
    }
    void SendWaveAnalytics(string eventType, float timeValue)
    {
        // ✅ เช็คตรง ๆ ว่า init แล้วหรือยัง
        if (UnityServices.State != ServicesInitializationState.Initialized)
            return;

        CustomEvent waveAnalytics = new CustomEvent("WaveAnalytics");
        waveAnalytics.Add("event_type", eventType);
        waveAnalytics.Add("wave", currentWave);
        waveAnalytics.Add("highest_wave", highestWave);
        waveAnalytics.Add("time", timeValue);
        waveAnalytics.Add("deaths", deathCountThisWave);

        AnalyticsService.Instance.RecordEvent(waveAnalytics);
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
    public int GetHighestWave()
    {
        return highestWave;
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
