using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableDoor : MonoBehaviour
{
    private bool isPlayerInRange; // Tracks if the player is near this door
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is in range and presses the "E" key
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Destroy(gameObject);
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
