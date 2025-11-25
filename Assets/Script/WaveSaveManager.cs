using UnityEngine;

public static class WaveSaveManager
{
    private static string waveKey = "SavedWave";

    // บันทึก Wave ปัจจุบัน
    public static void SaveWave(int wave)
    {
        PlayerPrefs.SetInt(waveKey, wave);
        PlayerPrefs.Save();
        Debug.Log("Wave Saved: " + wave);
    }

    // โหลด Wave (ถ้าไม่เคย Save ให้เริ่ม Wave 1)
    public static int LoadWave()
    {
        return PlayerPrefs.GetInt(waveKey, 1);
    }

    // ล้าง Save (ใช้ตอนเริ่มเกมใหม่)
    public static void ResetWave()
    {
        PlayerPrefs.DeleteKey(waveKey);
    }
}