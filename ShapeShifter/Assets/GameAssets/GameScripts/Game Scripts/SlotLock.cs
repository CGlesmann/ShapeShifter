using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SlotLock : MonoBehaviour
{
    public enum LockType { Switch, Destruct };

    [Header("Lock Settings")]
    public LockData lockData = null;

    private ILockState lockStateMachine;
    private bool minimized = false;
    private bool destroyed = false;

    [Header("Object References")]
    [SerializeField] private Animator anim = null;
    [SerializeField] private TextMeshProUGUI counterText = null;
    private GameSlot gameSlot = null;

    private void OnDisable() { DeactivateLock(); }
    private void Awake()
    {
        gameSlot = transform.parent.GetComponent<GameSlot>();
        SetLockStateMachine(lockData.lockType);
        ActivateLock();
    }

    public void ResetLockState() { minimized = false; anim.ResetTrigger("Activate"); }
    public void SetLockToActive() { anim.SetTrigger("Activate"); }
    public void SetLockToDestruct() { anim.SetTrigger("Destroy"); }

    public void ToggleLockSize()
    {
        if (minimized)
            ResizeLock();
        else
            MinimizeLock();
    }

    public void ResizeLock() { anim.SetTrigger("Resize"); }
    public void MinimizeLock()
    {
        if (!minimized)
        {
            anim.SetTrigger("Minimize");
            minimized = true;
        }
    }

    public LockType GetLockType() { return lockData?.lockType ?? LockType.Destruct; }
    public int GetLockCounter() { return lockData?.lockTimer ?? -1; }

    public void SetLockStateMachine(ILockState newStateMachine) { lockStateMachine = newStateMachine; }
    public void SetLockStateMachine(LockType lockType)
    {
        switch (lockType)
        {
            case LockType.Destruct:
                SetLockStateMachine(new DestructLockState(this));
                break;
            case LockType.Switch:
                SetLockStateMachine(new SwitchLockState(this));
                break;
        }
    }
    
    public void SetLockSettings(LockType lockType, int lockTimer)
    {
        if (lockType != lockData.lockType)
            SetLockStateMachine(lockType);

        if (lockData.lockTimer == 0)
            ActivateLock();

        lockData = new LockData(lockType, lockTimer);
        UpdateCounterText();
    }

    public void ActivateLock()
    {
        LockGameSlot();
        UpdateCounterText();
        lockStateMachine?.ActivateLock();
    }

    public void DeactivateLock()
    {
        lockStateMachine.DeactivateLock();
        gameSlot.UnlockGameSlot();
        anim.SetTrigger("Destroy");
    }

    public void LockGameSlot() { gameSlot?.LockGameSlot(); }
    public void UpdateLock() { lockStateMachine.UpdateLock(); }
    public void UpdateCounterText()
    {
        int newValue = (int)Mathf.Clamp(lockData.lockTimer, 0, Mathf.Infinity);
        counterText.text = newValue.ToString();
    }
}

[System.Serializable]
public class LockData
{
    public SlotLock.LockType lockType = SlotLock.LockType.Destruct;
    [Range(1, 99)] public int lockTimer = 0;

    public LockData(SlotLock.LockType lockType, int counter)
    {
        this.lockType = lockType;
        this.lockTimer = counter;
    }
}

public interface ILockState
{
    void ActivateLock();
    void DeactivateLock();
    void UpdateLock();
}

public class SwitchLockState : ILockState
{
    SlotLock slotLock;

    public SwitchLockState(SlotLock lockReference) { slotLock = lockReference; }
    public void ActivateLock() { GameManager.onShapeSwap += UpdateLock; }
    public void DeactivateLock() { GameManager.onShapeSwap -= UpdateLock; }

    public void UpdateLock()
    {
        slotLock.lockData.lockTimer--;
        slotLock.UpdateCounterText();

        if (slotLock.lockData.lockTimer <= 0)
            slotLock.DeactivateLock();
    }
}

public class DestructLockState : ILockState
{
    SlotLock slotLock;

    public DestructLockState(SlotLock lockReference) { slotLock = lockReference; }
    public void ActivateLock() { BoardManager.onShapeDestroy += MarkShapesDestroyed; }
    public void DeactivateLock() { BoardManager.onShapeDestroy -= MarkShapesDestroyed; }

    public void MarkShapesDestroyed(int count)
    {
        if (count > 0)
        {
            slotLock.lockData.lockTimer -= count;
            UpdateLock();
        }
    }

    public void UpdateLock()
    {
        slotLock.UpdateCounterText();
        if (slotLock.lockData.lockTimer <= 0)
            slotLock.DeactivateLock();
    }
}