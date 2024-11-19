using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    public Transform linkedDoor; // The other door to teleport to
    private bool isPlayerInRange; // Tracks if the player is near this door

    // Update is called once per frame
    void Update()
    {
        // Check if the player is in range and presses the "E" key
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        if (linkedDoor != null)
        {
            // Find the player and move them to the linked door's position
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = linkedDoor.position;
                Debug.Log("Player teleported to: " + linkedDoor.position);
            }
            else
            {
                Debug.LogError("Player GameObject not found! Is it tagged as 'Player'?");
            }
        }
        else
        {
            Debug.LogWarning("Linked door is not assigned!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the door's trigger area
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered the trigger area of: " + gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the player leaves the door's trigger area
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player exited the trigger area of: " + gameObject.name);
        }
    }
}
