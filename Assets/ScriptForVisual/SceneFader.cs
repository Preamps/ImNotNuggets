using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;
    public CanvasGroup fadeGroup;
    public float fadeSpeed = 1.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        fadeGroup.alpha = 1;

        while (fadeGroup.alpha > 0)
        {
            fadeGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        fadeGroup.interactable = false;
        fadeGroup.blocksRaycasts = false;
    }

    public IEnumerator FadeOut(string sceneName)
    {
        fadeGroup.interactable = true;
        fadeGroup.blocksRaycasts = true;

        fadeGroup.alpha = 0;

        while (fadeGroup.alpha < 1)
        {
            fadeGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
