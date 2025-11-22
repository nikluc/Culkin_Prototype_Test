using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Settings")]
    public int poolSize = 5; // Number of audio sources in the pool

    private List<AudioSource> audioSources;
    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        audioSources = new List<AudioSource>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioSources.Add(source);
        }
    }

    // Play an SFX clip through the next available audio source in the pool
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource source = audioSources[currentIndex];
        source.clip = clip;
        source.volume = volume;
        source.Play();

        currentIndex = (currentIndex + 1) % poolSize;
    }
}