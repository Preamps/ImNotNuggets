using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // --- Private Variables for Logic & Analytics ---
    private int currentWave = 0;
    private int highestWave = 0;
    private int deathsInCurrentWave = 0;
    private float waveStartTime = 0f;
    private bool waveActive = false;
    private bool isRestarting = false;
    private List<GameObject> aliveEnemies = new List<GameObject>();
   
    private IEnumerator Start()
    {
        // ตรวจว่าเป็น Start ใหม่ หรือ Restart
        deathsInCurrentWave = 0;
        while (!InitUGS.IsInitialized)
        {
            yield return null;
        }

        bool isNewGame = PlayerPrefs.GetInt("StartNewGame", 0) == 1;
        highestWave = PlayerPrefs.GetInt("HighestWave", 0);
        
        if (isNewGame)
        {
            currentWave = 0;
            highestWave = 0;
            WaveSaveManager.ResetWave();
            PlayerPrefs.SetInt("HighestWave", 0);
            PlayerPrefs.SetInt("StartNewGame", 0);
            PlayerPrefs.Save();
        }
        else
        {
            // Restart → โหลด wave ล่าสุด
            int savedWave = WaveSaveManager.LoadWave(); // เช่น 5
            currentWave = savedWave - 1; // -1 เพราะ wave จะถูก ++ ใน StartNextWave()
            
            deathsInCurrentWave = PlayerPrefs.GetInt("SavedDeathsCount", 0);
        }

        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        if (waveActive && aliveEnemies.Count == 0)
        {
            waveActive = false;

            SendWaveAnalytics("Passed");
            
            deathsInCurrentWave = 0;
            PlayerPrefs.SetInt("SavedDeathsCount", 0);
            PlayerPrefs.Save();
            StartCoroutine(StartNextWave());

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX("WaveStart");
        }
    }

    IEnumerator StartNextWave()
    {
        currentWave++;
    
        if (currentWave > totalWaves)
        {
            Debug.Log("<color=cyan>★★★ VICTORY: ALL WAVES COMPLETED! ★★★</color>");

            if (wavesUI != null)
                wavesUI.ShowWavesCompleteUI();

            if (waveText != null)
                waveText.text = "Victory!";

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX("Win");

            yield break; // จบการทำงาน ไม่ Spawn ศัตรูเพิ่ม
        }

        WaveSaveManager.SaveWave(currentWave);

        // อัปเดตสถิติ Wave สูงสุด
        if (currentWave > highestWave)
        {
            highestWave = currentWave;
            PlayerPrefs.SetInt("HighestWave", highestWave);
            PlayerPrefs.Save();
        }
        
        if (waveText != null)
            waveText.text = $"{currentWave} / {totalWaves}";

        yield return new WaitForSeconds(timeBetweenWaves);

        // เริ่มจับเวลา Wave และ Spawn ศัตรู
        waveStartTime = Time.time;
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
        deathsInCurrentWave++;
        // บันทึกลงเครื่องทันที เผื่อกด Restart Scene ค่ายังคงอยู่และนับต่อได้
        
        PlayerPrefs.SetInt("SavedDeathsCount", deathsInCurrentWave);
        PlayerPrefs.Save();
        Debug.Log($"<color=red>Player Died!</color> Wave: {currentWave} | Deaths this wave: {deathsInCurrentWave}");
        
    }
    public void RestartWave()
    {
        isRestarting = true;
        // บันทึกจำนวนการตายที่เพิ่มขึ้นก่อนโหลดฉากใหม่
        PlayerPrefs.SetInt("SavedDeathsCount", deathsInCurrentWave);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SendWaveAnalytics(string status)
    {
        if (!InitUGS.IsInitialized) return;
        float duration = Time.time - waveStartTime;
        double formattedDuration = System.Math.Round((double)duration, 2);

        AnalyticsService.Instance.StartDataCollection();

        CustomEvent waveEvent = new CustomEvent("Wave_Report");
        waveEvent.Add("Wave_Current", currentWave);           // Wave ปัจจุบัน
        waveEvent.Add("Status", status);                      // Passed หรือ Failed
        waveEvent.Add("Time_Spent", formattedDuration);       // ใช้เวลากี่วินาที (ทศนิยม 2 ตำแหน่ง)
        waveEvent.Add("Deaths_In_Wave", deathsInCurrentWave); // ตายใน Wave นี้กี่ครั้ง
        waveEvent.Add("Highest_Wave_Reached", highestWave);   // ไปถึง Wave ไหนสูงสุด

        try
        {
            AnalyticsService.Instance.RecordEvent(waveEvent);
            // ใน Editor ต้อง Flush บ่อยหน่อยเพื่อให้ข้อมูลไปปรากฏใน Dashboard เร็วขึ้น
            AnalyticsService.Instance.Flush();
            Debug.Log($"<color=gold>Analytics Sent:</color> Wave {currentWave} | Time: {formattedDuration}s | Deaths: {deathsInCurrentWave}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Analytics Error: " + e.Message);
        }
    }
    private void OnDisable()
    {
        if (waveActive && !isRestarting && Application.isEditor)
        {
            SendWaveAnalytics("Player_Quit");
        }
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
