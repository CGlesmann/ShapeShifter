using System;
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

    private Action onLockDestroy = null;
    private ILockState lockStateMachine;

    private bool minimized = false;
    private bool animating = false;

    [Header("Animation Settings")]
    [SerializeField] private float resizeTime = 1f;
    [SerializeField] private Transform counterBack = null;
    [SerializeField] private Vector2 counterFullPosition = Vector2.zero;
    [SerializeField] private Vector2 counterMinimizedPosition = Vector2.zero;
    private float baseSize = 0f;

    [Header("Object References")]
    [SerializeField] private Animator anim = null;
    [SerializeField] private LockCounterText textAnim = null;
    private GameSlot gameSlot = null;

    private void OnDisable() { DeactivateLock(); }
    private void Awake()
    {
        baseSize = GetComponent<RectTransform>().rect.height * transform.localScale.y;

        gameSlot = transform.parent.GetComponent<GameSlot>();
        SetLockStateMachine(lockData.lockType);
        ActivateLock();
    }

    public void ResetLockState() { minimized = false; anim.ResetTrigger("Activate"); }
    public void SetLockToActive() { anim.SetTrigger("Activate"); }
    public void SetLockToDestruct() { anim.SetTrigger("Destroy"); }

    public void ToggleLockSize()
    {
        if (!animating)
        {
            if (minimized)
                ResizeLock();
            else
                MinimizeLock();
        }
    }

    public void ResizeLock()
    {
        anim.SetTrigger("Resize");
        animating = true;

        StartCoroutine(LerpToPosition(new Vector2(0, -baseSize / 2),
                                      Vector2.zero,
                                      counterMinimizedPosition,
                                      counterFullPosition
                                      ));
    }

    public void MinimizeLock()
    {
        if (!minimized)
        {
            anim.SetTrigger("Minimize");
            minimized = true;
            animating = true;

            StartCoroutine(LerpToPosition(Vector2.zero, 
                                          new Vector2(0, -baseSize / 2),
                                          counterFullPosition,
                                          counterMinimizedPosition
                                          ));
        }
    }

    private IEnumerator LerpToPosition(Vector2 lockStartPosition, Vector2 lockTargetPosition, Vector2 counterStartPosition, Vector2 counterTargetPosition)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * resizeTime;
            transform.localPosition = Vector2.Lerp(lockStartPosition, lockTargetPosition, progress);
            counterBack.localPosition = Vector2.Lerp(counterStartPosition, counterTargetPosition, progress);

            yield return null;
        }

        animating = false;
    }

    public LockType GetLockType() { return lockData?.lockType ?? LockType.Destruct; }
    public int GetLockCounter() { return lockData?.lockTimer ?? -1; }

    public void LockGameSlot() { gameSlot?.LockGameSlot(); }
    public void UpdateLock() { lockStateMachine.UpdateLock(); }

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

        if (lockData.lockTimer <= 0)
            ActivateLock();

        if (lockData == null)
            lockData = new LockData(lockType, lockTimer);
        else
        {
            lockData.lockType = lockType;
            lockData.lockTimer = lockTimer;
        }

        textAnim.SetTextValue(lockData.lockTimer);
    }

    public void ActivateLock()
    {
        LockGameSlot();
        textAnim?.SetTextValue(lockData.lockTimer);
        lockStateMachine?.ActivateLock();
    }

    public void DeactivateLock()
    {
        lockStateMachine.DeactivateLock();
        gameSlot.UnlockGameSlot();
        anim.SetTrigger("Destroy");
    }

    public void DestroyLock()
    {
        onLockDestroy?.Invoke();
        onLockDestroy = null;
    }

    public void PlayTextUpdateAnimation()
    {
        int newValue = (int)Mathf.Clamp(lockData.lockTimer, 0, Mathf.Infinity);

        Action animationFinishAction = BoardManager.boardManager.MarkLockAsAnimating();
        if (newValue > 0)
            textAnim.PlayUpdateAnimation(newValue, animationFinishAction);
        else
        {
            onLockDestroy += animationFinishAction;
            textAnim.PlayUpdateAnimation(newValue, null);
        }
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
    public void ActivateLock() { BoardManager.boardManager.onShapeSwap += UpdateLock; }
    public void DeactivateLock() { BoardManager.boardManager.onShapeSwap -= UpdateLock; }

    public void UpdateLock()
    {
        slotLock.lockData.lockTimer--;

        slotLock.PlayTextUpdateAnimation();
        if (slotLock.lockData.lockTimer <= 0)
            slotLock.DeactivateLock();
    }
}

public class DestructLockState : ILockState
{
    SlotLock slotLock;

    public DestructLockState(SlotLock lockReference) { slotLock = lockReference; }
    public void ActivateLock() { Debug.Log($"Activating lock {slotLock.name}"); BoardManager.boardManager.onShapeDestroy += MarkShapesDestroyed; }
    public void DeactivateLock() { BoardManager.boardManager.onShapeDestroy -= MarkShapesDestroyed; }

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
        slotLock.PlayTextUpdateAnimation();
        if (slotLock.lockData.lockTimer <= 0)
            slotLock.DeactivateLock();
    }
}