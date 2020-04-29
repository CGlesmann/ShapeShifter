using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LockCounterText : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Animator anim = null;
    [SerializeField] private TextMeshProUGUI counterText = null;

    /*
    public delegate void OnLockAnimationFinish();
    private event OnLockAnimationFinish onLockAnimationFinish = null;
    */

    private Action onLockAnimationFinish = null;
    private int value;

    public void PlayUpdateAnimation(int newValue, Action finishEvent)
    {
        anim.SetTrigger("Update");
        onLockAnimationFinish = finishEvent;
        value = newValue;
    }

    public void SetTextValue(int value) { counterText.text = value.ToString(); }
    public void UpdateCounter() { counterText.text = value.ToString(); }
    
    public void ExecuteAnimationFinish()
    {
        onLockAnimationFinish?.Invoke();
        onLockAnimationFinish = null;
    }
}
