using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackButton : MonoBehaviour
{
    [Header("Pack Settings")]
    [SerializeField] private bool requireCompletionToUnlock = true;
    [SerializeField] private int packIndex = 0;
    [SerializeField] private int minRequiredLevel = 0;

    [Header("Object References")]
    [SerializeField] private DynamicGeneralThemeElement themeElement = null;
    private Animator anim;

    private void OnEnable() { anim = GetComponent<Animator>(); }

    public void SetLockState() { themeElement.SetElementToHighlighted(); }
    public void SetUnlockState() { themeElement.SetElementToNormal(); anim.SetBool("Unlocked", true); }
    public void TriggerUnlock() { themeElement.SetElementToHighlighted(); anim.SetTrigger("Unlock"); }
    public bool CheckForUnlock()
    {
        if (DataTracker.gameData.completedLevels.TryGetValue(packIndex, out int highestLevelCompleted))
        {
            int highestPackUnlocked = DataTracker.gameData.highestPackUnlocked;

            if (highestLevelCompleted >= minRequiredLevel)
            {
                if (highestPackUnlocked >= packIndex)
                    SetUnlockState();
                else
                    return true;
            } else
                SetLockState();
        }
        else if (!requireCompletionToUnlock)
            SetUnlockState();
        else
            SetLockState();

        return false;
    }
}
