using UnityEngine;
using System.Collections;

public class HpSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs;  // พรีแฟบไอเท็มเจ็ม
    public float minX = -18f;
    public float maxX = 12f;
    public float minY = -12f;
    public float maxY = 20f;
    public float spawnInterval = 20f;

    public int maxItems = 15;   // ⭐ จำนวนสูงสุดของ item ในซีน
    public int spawnCount = 5;
    // เรียกได้จาก WaveManager หรือเรียกเองก็ได้
    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }
    
    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnItems();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    public void SpawnItems()
    {
        if (itemPrefabs.Length == 0)
        {
            Debug.LogWarning("MultiItemSpawner2D: ไม่มี itemPrefabs");
            return;
        }

        // ⭐ นับจำนวนไอเท็มในซีนตอนนี้
        int currentItemCount = GameObject.FindGameObjectsWithTag("ItemHP").Length;

        // ⭐ ถ้าเกิน 15 หยุด spawn
        if (currentItemCount >= maxItems)
        {
            Debug.Log("Items เต็มจำนวนแล้ว ไม่ spawn เพิ่ม");
            return;
        }

        // ⭐ คำนวณจำนวนที่ spawn ได้ (ห้ามทำให้เกิน 15)
        int amountToSpawn = Mathf.Min(spawnCount, maxItems - currentItemCount);

        for (int i = 0; i < spawnCount; i++)
        {
            // สุ่มไอเท็มจาก array
            GameObject randomItem = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

            // สุ่มตำแหน่ง X,Y ในขอบเขตที่กำหนด
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);

            Vector3 spawnPos = new Vector3(x, y, 0f); // Z = 0 สำหรับ 2D

            // สร้างไอเท็ม
            Instantiate(randomItem, spawnPos, Quaternion.identity);
        }
    }
}
