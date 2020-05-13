using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnlockNotification : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TextMeshProUGUI notificationText = null;
    [SerializeField] private Animator anim = null;

    public delegate void OnNotificationDestroy();
    private event OnNotificationDestroy onNotificationDestroy;

    public void SubscribeEventToDestroy(OnNotificationDestroy action) { onNotificationDestroy += action; }
    public void UpdateNotificationText(string str) { notificationText.text = str; }
    public void DismissNotification() { anim.SetTrigger("Dismiss"); }
    public void DestroyNotification() { onNotificationDestroy?.Invoke(); GameObject.Destroy(gameObject); }
}
