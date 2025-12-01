using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
   
    public int healAmount = 20;          // Amount of HP restored
    public float rotationSpeed = 50f;    // Just for visual spinning effect (optional)
           

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

            SoundManager.Instance.PlaySFX("HPpickup");





            // Destroy the item after pickup
            Destroy(gameObject);
        }
    }
}
