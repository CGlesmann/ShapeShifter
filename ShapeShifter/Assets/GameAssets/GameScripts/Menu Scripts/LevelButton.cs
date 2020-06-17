using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Button Settings")]
    public bool requireLevelUnlock = true;
    [SerializeField] private int buttonIndex = 0;

    [Header("Object References")]
    [SerializeField] private LevelSelectManager manager = null;
    [SerializeField] private Animator anim = null;
    [SerializeField] private GameObject completionIcon = null;
    [SerializeField] private DynamicGeneralThemeElement themeElement = null;

    private string levelName => string.Format("Level_{0}", buttonIndex + 1);
    [SerializeField] private bool locked = false;

    public Action SetLevelButtonState(int highestLevelUnlock, int highestDisplay, out bool displayUnlock)
    {
        if (buttonIndex <= highestLevelUnlock)
        {
            if (buttonIndex > highestDisplay)
            {
                if (requireLevelUnlock)
                {
                    displayUnlock = true;
                    SetLockDisplay();
                    return DisplayUnlockAnimation;
                }
                else
                {
                    displayUnlock = false;
                    SetUnlockDisplay();
                    return null;
                }
            }
            else
            {
                displayUnlock = false;
                SetUnlockDisplay();
                return null;
            }
        }

        displayUnlock = false;
        SetLockDisplay();
        return null;
    }

    public void SelectLevel()
    {
        if (!locked)
            manager.DisplayLevelPreview(levelName, buttonIndex);
    }

    public void UnlockLevelButton()
    {
        locked = false;
        GetComponent<Button>().interactable = true;

        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        ChallengeLog challengeLog = ChallengeManager.GetCurrentChallengeLog(manager.levelPackIndex + 1, buttonIndex + 1);

        Dictionary<int, bool> completedChallenges = saveDataAccessor.GetDataValue<Dictionary<int, bool>>(SaveKeys.COMPLETED_CHALLENGES_SAVE_KEY);
        if (challengeLog != null && completedChallenges != null)
        {
            for(int i = 0; i < challengeLog.GetChallengeCount(); i++)
            {
                int challengeKey = Challenge.GetChallengeKey(manager.levelPackIndex + 1, buttonIndex + 1, i);
                if (completedChallenges.TryGetValue(challengeKey, out bool challengeResult))
                {
                    if (!challengeResult)
                    {
                        completionIcon.SetActive(false);
                        return;
                    }
                }
                else
                {
                    completionIcon.SetActive(false);
                    return;
                }
            }

            completionIcon.SetActive(true);
        }
        else
            completionIcon.SetActive(false);
    }

    private void LockLevelButton()
    {
        locked = true;
        GetComponent<Button>().interactable = false;
    }

    public void DisplayUnlockAnimation() { themeElement.SetElementToHighlighted(); UnlockLevelButton(); anim.SetTrigger("Unlock"); }
    public void SetLockDisplay() { themeElement.SetElementToHighlighted(); LockLevelButton(); anim.SetBool("Unlocked", false); }
    public void SetUnlockDisplay() { themeElement.SetElementToNormal(); UnlockLevelButton(); anim.SetBool("Unlocked", true); }
    public int GetIndex() { return buttonIndex; }
}
