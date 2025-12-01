using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // ตั้ง flag ว่าเป็นเกมใหม่ → Wave 1
        PlayerPrefs.SetInt("StartNewGame", 1);

        StartCoroutine(SceneFader.Instance.FadeOut("GamePlay"));
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
