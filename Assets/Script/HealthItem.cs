using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [Header("Heal Settings")]
    public int healAmount = 20;          // Amount of HP restored
    public float rotationSpeed = 50f;    // Just for visual spinning effect (optional)
    public AudioClip pickupSound;        

    private void Update()
    {
        // Optional: rotate the item for visibility
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            // Heal player
            player.Health += healAmount;

            // Prevent overhealing (assuming max HP = 100)
            if (player.Health > 100)
                player.Health = 100;

            // Update the health bar UI
            if (player.healthBar != null)
                player.healthBar.UpdateHealthBar(player.Health);

            // Play sound effect
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            // Destroy the item after pickup
            Destroy(gameObject);
        }
    }
}
