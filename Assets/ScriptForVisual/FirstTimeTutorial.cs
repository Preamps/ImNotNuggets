using UnityEngine;

public class FirstTimeTutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    private static bool hasShownTutorial = false; // session-wide

    void Start()
    {
        if (!hasShownTutorial)
        {
            ShowTutorial();
            hasShownTutorial = true;
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }

    void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        Time.timeScale = 0f; // pause game
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f; // resume game
    }
}
