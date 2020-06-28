using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour
{
    [Header("Sound References")]
    [SerializeField] protected AudioClip pressSoundEffect = null;

    public void PlayPressSound() { SoundManager.soundManager.PlaySoundEffect(pressSoundEffect); }
}
