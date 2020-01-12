using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SlotLock : MonoBehaviour
{
    public enum LockType { Timer, Destruct };

    [Header("Lock Settings")]
    [SerializeField] private bool locked = false;
    private LockType lockType { get; set; }

    [Header("Timer Lock Settings")]
    [SerializeField] private int turnTimer = 0;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText = null;

    private void Awake()
    {
        if (locked)
            ActivateLock();
    }

    private void OnDisable()
    {
        if (locked)
            DeactivateLock();
    }

    public bool IsLocked() { return locked; }

    public void ActivateLock()
    {
        if (lockType == LockType.Timer)
            GameManager.onShapeSwap += UpdateLock;

        GameManager.onUpdateBoard += DestroyLock;
    }

    public void DeactivateLock()
    {
        if (lockType == LockType.Timer)
            GameManager.onShapeSwap -= UpdateLock;

        GameManager.onUpdateBoard -= DestroyLock;
    }

    public void DestroyLock()
    {
        if (!locked)
        {
            DeactivateLock();
            GameObject.Destroy(gameObject);
        }
    }

    public void UpdateLock()
    {
        if (lockType == LockType.Timer)
        {
            turnTimer--;
            if (turnTimer == 0)
                locked = false;
            else
                timerText.text = turnTimer.ToString();
        } 
    }
};