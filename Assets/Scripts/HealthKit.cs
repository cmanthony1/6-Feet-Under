using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthKit : MonoBehaviour
{
    public float healthRestoreAmount = 5f; // How much health to restore
    private bool playerInRange = false;    // Tracks if the player is nearby

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.RestoreHealth(healthRestoreAmount);
                Debug.Log("Health restored!");

                Destroy(gameObject); // Remove crate after use
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Press 'E' to restore health.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
