using UnityEngine;
using UnityEngine.SceneManagement;

public class WavesCompleteUIManager : MonoBehaviour
{
    public GameObject wavesCompleteUI;

    private void Start()
    {
        wavesCompleteUI.SetActive(false);
    }

    public void ShowWavesCompleteUI()
    {
        wavesCompleteUI.SetActive(true);
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
