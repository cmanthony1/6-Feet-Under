using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteraction : MonoBehaviour
{
    [SerializeField] private string targetScene; // Scene to load

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        // Ensure the BoxCollider2D is set as a trigger
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider2D is missing on the door object!");
        }
        else
        {
            boxCollider.isTrigger = true; // Ensure it's a trigger
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"Loading scene: {targetScene}");
            if (!string.IsNullOrEmpty(targetScene))
            {
                SceneManager.LoadScene(targetScene);
            }
            else
            {
                Debug.LogWarning("Target scene is not set.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the door trigger zone.");
        }
    }
}
