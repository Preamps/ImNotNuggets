using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(SceneFader.Instance.FadeOut("GamePlay"));
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
