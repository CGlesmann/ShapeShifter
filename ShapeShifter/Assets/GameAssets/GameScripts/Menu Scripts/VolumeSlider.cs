using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private AudioMixer targetMixer = null;
    [SerializeField] private Slider slider = null;

    private void Awake()
    {
        if (targetMixer.GetFloat("Volume", out float currentValue))
            slider.value = currentValue;
    }

    public void AdjustAudio(Slider obj) { targetMixer.SetFloat("Volume", obj.value); }
}
