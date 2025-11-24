using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private RectTransform fillRect; // Assign the Fill area
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeAmount = 5f;

    private Vector2 originalPos;

    private void Awake()
    {
        if (fillRect == null)
            fillRect = healthSlider.fillRect;
        originalPos = fillRect.anchoredPosition;
    }

    public void SetMaxHealth(int health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
    }

    public void UpdateHealthBar(int health)
    {
        if (health < healthSlider.value)
        {
            StopAllCoroutines();
            StartCoroutine(ShakeHealthBar());
        }

        healthSlider.value = health;
    }

    private IEnumerator ShakeHealthBar()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-shakeAmount, shakeAmount);
            float y = Random.Range(-shakeAmount, shakeAmount);
            fillRect.anchoredPosition = originalPos + new Vector2(x, y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        fillRect.anchoredPosition = originalPos;
    }
}
