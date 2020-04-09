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
    [SerializeField] private float revealTime = 2f;
    public LockData lockData = null;

    private ILockState lockStateMachine;
    private bool faded = false;
    private float revealTimer = 0;

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

    private void Update()
    {
        if (revealTimer > 0)
        {
            revealTimer -= Time.deltaTime;
            if (revealTimer <= 0f)
                RevealLockImage();
        }
    }

    public void FadeLockImage()
    {
        if (!faded)
        {
            anim.SetTrigger("Disappear");
            faded = true;
        }
    }

    public LockType GetLockType() { return lockData?.lockType ?? LockType.Destruct; }
    public int GetLockCounter() { return lockData?.lockTimer ?? -1; }

    public void SetLockToDestruct() { anim.SetTrigger("ImmediateDestruct"); }
    public void SetLockToActive() { anim.SetTrigger("ImmediateActive"); }

    public void SetLockTimer() { revealTimer = revealTime; }
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

    public void RevealLockImage() { anim.SetTrigger("Reappear"); }
    public void ResetLockState() { faded = false; revealTimer = 0; }

    public void ActivateLock()
    {
        LockGameSlot();
        UpdateCounterText();
        lockStateMachine.ActivateLock();
    }

    public void DeactivateLock()
    {
        lockStateMachine.DeactivateLock();
        gameSlot.UnlockGameSlot();
        anim.SetTrigger("Destroy");
    }

    public void LockGameSlot() { gameSlot.LockGameSlot(); }
    public void UpdateLock() { lockStateMachine.UpdateLock(); }
    public void UpdateCounterText() { counterText.text = lockData.lockTimer.ToString(); }
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
        if (slotLock.lockData.lockTimer == 0)
            slotLock.DeactivateLock();
        else
            slotLock.UpdateCounterText();
    }
}

public class DestructLockState : ILockState
{
    SlotLock slotLock;

    public DestructLockState(SlotLock lockReference) { slotLock = lockReference; }
    public void ActivateLock() { GameManager.onShapeDestroy += MarkShapesDestroyed; }
    public void DeactivateLock() { GameManager.onShapeDestroy -= MarkShapesDestroyed; }

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
        if (slotLock.lockData.lockTimer == 0)
            slotLock.DeactivateLock();
        else
            slotLock.UpdateCounterText();
    }
}