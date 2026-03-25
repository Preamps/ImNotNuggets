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

    // Analytics Data
    private int totalDeathCount = 0;    // นับสะสมต่อเนื่อง
    private int highestWave = 0;        // Wave สูงสุดที่ไปถึง
    private float waveStartTime = 0f;

    private IEnumerator Start()
    {
        // ตรวจว่าเป็น Start ใหม่ หรือ Restart
        while (!InitUGS.IsInitialized)
        {
            yield return null;
        }

        bool isNewGame = PlayerPrefs.GetInt("StartNewGame", 0) == 1;
        highestWave = PlayerPrefs.GetInt("HighestWave", 0);
        totalDeathCount = PlayerPrefs.GetInt("TotalDeaths", 0);

        if (isNewGame)
        {
            currentWave = 0;
            highestWave = 0;
            totalDeathCount = 0;
            WaveSaveManager.ResetWave();
            PlayerPrefs.SetInt("HighestWave", 0);
            PlayerPrefs.SetInt("TotalDeaths", 0);
            PlayerPrefs.SetInt("StartNewGame", 0);
        }
        else
        {
            // Restart → โหลด wave ล่าสุด
            int savedWave = WaveSaveManager.LoadWave(); // เช่น 5
            currentWave = savedWave - 1; // -1 เพราะ wave จะถูก ++ ใน StartNextWave()
        }

        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        if (waveActive && aliveEnemies.Count == 0)
        {
            waveActive = false;
            StartCoroutine(StartNextWave());

            if (SoundManager.Instance != null)
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

        // อัปเดตสถิติ Wave สูงสุด
        if (currentWave > highestWave)
        {
            highestWave = currentWave;
            PlayerPrefs.SetInt("HighestWave", highestWave);
            PlayerPrefs.Save();
        }

        Debug.Log("Wave " + currentWave + " is starting...");

        if (waveText != null)
            waveText.text = currentWave + " / " + totalWaves;

        // Reset Analytics
        waveStartTime = Time.time;
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
        if (aliveEnemies.Contains(enemy))
            aliveEnemies.Remove(enemy);
    }

    // Player เรียกตอนตาย
    public void RegisterDeath()
    {
        totalDeathCount++;
        PlayerPrefs.SetInt("TotalDeaths", totalDeathCount);
        PlayerPrefs.Save();
        Debug.Log("<color=red>Player Died!</color> Total Deaths: " + totalDeathCount);
    }

    public void SendPlayerSummary()
    {
        if (!InitUGS.IsInitialized) return;
        AnalyticsService.Instance.StartDataCollection();
        CustomEvent summaryEvent = new CustomEvent("Player_Summary");

        summaryEvent.Add("Player_Skill_Progression", highestWave);
        summaryEvent.Add("Time_to_Complete_Rate", Time.time);
        summaryEvent.Add("Failure_Rate", totalDeathCount);

        try
        {
            AnalyticsService.Instance.RecordEvent(summaryEvent);
            AnalyticsService.Instance.Flush();
            Debug.Log("<color=gold>Analytics Sent using your specific names!</color>");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Analytics Summary failed: " + e.Message);
        }
    }
    private void OnDisable()
    {
        if (Application.isEditor)
        {
            SendPlayerSummary();
        }
    }

    private void OnApplicationQuit()
    {
        SendPlayerSummary(); // ส่งข้อมูลเมื่อผู้เล่นปิดแอป/เลิกเล่น
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
