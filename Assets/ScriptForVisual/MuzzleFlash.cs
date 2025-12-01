using System.Collections;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public SpriteRenderer flashRenderer;  // Assign your muzzle sprite in inspector
    public float flashDuration = 0.05f;
    public bool randomRotation = true;    // Optional random rotation

    void Awake()
    {
        // Safety check
        if (flashRenderer == null)
            flashRenderer = GetComponent<SpriteRenderer>();

        if (flashRenderer != null)
            flashRenderer.enabled = false;
        else
            Debug.LogWarning("MuzzleFlash: No SpriteRenderer assigned!");
    }

    public void Flash()
    {
        if (flashRenderer != null)
            StartCoroutine(DoFlash());
    }

    private IEnumerator DoFlash()
    {
        flashRenderer.enabled = true;

        // Apply random rotation if enabled
        if (randomRotation)
            transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        yield return new WaitForSeconds(flashDuration);

        flashRenderer.enabled = false;
    }
}
