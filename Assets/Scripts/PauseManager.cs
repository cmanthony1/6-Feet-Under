using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused = false;

    void Start()
    {
        pauseMenu.SetActive(false); // Hide pause menu on start
        Time.timeScale = 1f; // Ensure game starts unpaused
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }

        // Notify player script
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.SetPaused(isPaused);
        }
    }
}

