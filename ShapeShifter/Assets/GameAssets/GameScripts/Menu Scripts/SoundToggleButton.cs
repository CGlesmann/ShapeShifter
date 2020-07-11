using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundToggleButton : MonoBehaviour
{
    private const string volumeVariableName = "Volume";

    [Header("Mixer References")]
    [SerializeField] private AudioMixer audioMixer = null;

    [Header("Toggle Settings")]
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 0f;

    public void ToggleSound()
    {
        if (audioMixer.GetFloat(volumeVariableName, out float currentSoundValue))
        {
            
        }
    }
}
