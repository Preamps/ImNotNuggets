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
