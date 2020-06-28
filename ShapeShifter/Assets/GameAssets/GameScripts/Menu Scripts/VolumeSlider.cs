using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private AudioMixer targetMixer = null;

    public void AdjustAudio(Slider obj) { targetMixer.SetFloat("Volume", obj.value); }
}
