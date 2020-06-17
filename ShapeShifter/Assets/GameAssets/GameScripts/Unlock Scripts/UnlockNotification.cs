using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;

public class UnlockNotification : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TextMeshProUGUI notificationText = null;
    [SerializeField] private DynamicGeneralThemeElement backThemeElement = null;
    [SerializeField] private Animator anim = null;

    [Header("Animation Settings")]
    [SerializeField] private Transform animTarget = null;
    [SerializeField] private Vector2 enterPosition = Vector2.zero;
    [SerializeField] private Vector2 idlePosition = Vector2.zero;
    [SerializeField] private Vector2 exitPosition = Vector2.zero;
    [SerializeField] private float animationTime = 0f;

    private bool animating = false;
    private bool unlocked = false;
     
    public delegate void OnNotificationDestroy();
    private event OnNotificationDestroy onNotificationDestroy;

    private delegate void OnLerpFinish();

    private void Awake() { ExecuteEnterAnimation(); animating = true; }

    public void ExecuteEnterAnimation() { StartCoroutine(LerpPosition(enterPosition, idlePosition, animationTime, SetAnimationFinish)); }
    public void ExecuteExitAnimation() { StartCoroutine(LerpPosition(idlePosition, exitPosition, animationTime, DestroyNotification)); }
    private IEnumerator LerpPosition(Vector2 start, Vector2 end, float time, OnLerpFinish callback)
    {
        float timer = 0f, inc;

        while(timer < 1f)
        {
            inc = Time.deltaTime / time;
            timer += inc;

            animTarget.localPosition = Vector2.Lerp(start, end, Mathf.SmoothStep(0f, 1f, timer));
            yield return null;
        }

        callback?.Invoke();
    }

    public void SubscribeEventToDestroy(OnNotificationDestroy action) { onNotificationDestroy += action; }
    public void UpdateNotificationText(string str) { notificationText.text = str; }
    public void ToggleNotificationState()
    {
        if (!animating)
        {
            if (!unlocked)
            {
                PlayUnlockNotification();
                unlocked = true;
                animating = true;
            }
            else
            {
                PlayExitAnimation();
                ExecuteExitAnimation();
                animating = true;
            }
        }
    }
    public void PlayUnlockNotification() { anim.SetTrigger("Unlock"); }
    public void PlayExitAnimation() { anim.SetTrigger("Dismiss"); }
    public void DestroyNotification() { onNotificationDestroy?.Invoke(); GameObject.Destroy(gameObject); }
    public void SetAnimationFinish() { animating = false; }
    public void SetUnlockState() 
    { 
        backThemeElement.SetElementToHighlighted(); 
        notificationText.gameObject.SetActive(true); 
    }
}
