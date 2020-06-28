using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager = null;

    [Header("Component References")]
    [SerializeField] private AudioSource musicAudioSource = null;
    [SerializeField] private AudioSource soundEffectAudioSource = null;

    private void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            GameObject.Destroy(gameObject);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicAudioSource.isPlaying)
            musicAudioSource.Stop();

        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        if (soundEffectAudioSource.isPlaying)
            soundEffectAudioSource.Stop();

        soundEffectAudioSource.clip = clip;
        soundEffectAudioSource.Play();
    }
}
