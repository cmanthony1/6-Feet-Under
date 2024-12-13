using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAudioPlayer : MonoBehaviour
{
    [Header("Audio Clips")]
    [Tooltip("The first audio clip to play when the scene loads.")]
    public AudioClip audioClip1;

    [Tooltip("The second audio clip to play when the scene loads (optional).")]
    public AudioClip audioClip2;

    [Header("Audio Settings")]
    [Tooltip("The volume of the first audio clip.")]
    [Range(0f, 1f)]
    public float volume1 = 1f;

    [Tooltip("The volume of the second audio clip.")]
    [Range(0f, 1f)]
    public float volume2 = 1f;

    private AudioSource audioSource1;
    private AudioSource audioSource2;

    void Start()
    {
        // Add AudioSource components if they don't already exist.
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();

        // Play the audio clips.
        PlayAudioClips();
    }

    private void PlayAudioClips()
    {
        if (audioClip1 != null)
        {
            audioSource1.clip = audioClip1;
            audioSource1.volume = volume1;
            audioSource1.Play();
        }

        if (audioClip2 != null)
        {
            audioSource2.clip = audioClip2;
            audioSource2.volume = volume2;
            audioSource2.Play();
        }
    }
}

