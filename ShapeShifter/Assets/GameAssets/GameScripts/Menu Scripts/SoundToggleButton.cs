using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundToggleButton : MonoBehaviour
{
    [Header("Mixer References")]
    [SerializeField] private AudioMixer audioMixer = null;

    [Header("Toggle Settings")]
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 0f;
}
