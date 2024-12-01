using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteraction : MonoBehaviour
{
    public string sceneToLoad; // Target scene name
    private bool isPlayerNearDoor = false; // Tracks if the player is near the door

    private void Update()
    {
        // Detect 'E' key press when near the door
        if (isPlayerNearDoor && Input.GetKeyDown(KeyCode.E))
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"Transitioning to scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene name not set for this door!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearDoor = true;
            Debug.Log("Player is near the door. Press 'E' to enter.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearDoor = false;
            Debug.Log("Player left the door area.");
        }
    }
}
