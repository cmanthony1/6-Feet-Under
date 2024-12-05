using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCrate : MonoBehaviour
{
    public int refillAmount = 6; // Amount of ammo to refill in the chamber
    public int reserveRefillAmount = 6; // Amount of ammo to refill in reserve

    private bool playerInRange = false; // Track if the player is in range to interact

    private void Update()
    {
        // When the player is in range, press 'E' to refill ammo
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Player player = FindObjectOfType<Player>(); // Find the player in the scene
            if (player != null)
            {
                player.RefillAmmo(refillAmount, reserveRefillAmount); // Refill the player's ammo
                Debug.Log("Ammo refilled!");

                Destroy(gameObject); // Destroy the ammo crate after refilling
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the player enters the crate's area
        {
            playerInRange = true;
            Debug.Log("Press 'E' to refill ammo.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the player exits the crate's area
        {
            playerInRange = false;
        }
    }
}
