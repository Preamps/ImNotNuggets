using UnityEngine;
using TMPro;

public class WaveUIManager : MonoBehaviour
{
    public TextMeshProUGUI waveText; // Assign in Inspector
    public WaveManager waveManager;   // Assign in Inspector

    private void Start()
    {
        UpdateWaveText(1, waveManager.totalWaves);
    }

    public void UpdateWaveText(int currentWave, int totalWaves)
    {
        waveText.text = $"Wave {currentWave}/{totalWaves}";
    }
}
