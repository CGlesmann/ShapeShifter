using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PackButton : GameButton
{
    [Header("Pack Settings")]
    [SerializeField] private bool requireUnlock = true;
    [SerializeField] private int minRequiredLevel = 0;

    [Header("Object References")]
    [SerializeField] private DynamicGeneralThemeElement themeElement = null;
    [SerializeField] private TextMeshProUGUI completionText = null;
    [SerializeField] private Animator anim = null;

    public void SetLockState() { themeElement.SetElementToHighlighted(); }
    public void SetUnlockState() { SetCompletionPercentage(); themeElement.SetElementToNormal(); anim.SetBool("Unlocked", true); }
    public void TriggerUnlock() { SetCompletionPercentage(); themeElement.SetElementToHighlighted(); anim.SetTrigger("Unlock"); }

    public Action CheckForUnlock(int highestCompletedPackLevel, int hightestDisplayedPackUnlock, out bool displayUnlock)
    {
        if (highestCompletedPackLevel >= minRequiredLevel)
        {
            if (transform.GetSiblingIndex() > hightestDisplayedPackUnlock)
            {
                if (requireUnlock)
                {
                    displayUnlock = true;
                    SetLockState();
                    return TriggerUnlock;
                }
                else
                {
                    displayUnlock = false;
                    SetUnlockState();
                    return null;
                }
            }
            else
            {
                displayUnlock = false;
                SetUnlockState();
                return null;
            }
        }

        displayUnlock = false;
        SetLockState();
        return null;
    }

    public void SetCompletionPercentage()
    {
        int packIndex = transform.GetSiblingIndex();
        ChallengeLog[] packLogs = Resources.LoadAll<ChallengeLog>($"ChallengeLogs/Level_Pack_{packIndex + 1}/");

        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        Dictionary<int, bool> completedChallenges = saveDataAccessor.GetDataValue<Dictionary<int, bool>>(SaveKeys.COMPLETED_CHALLENGES_SAVE_KEY);

        float amountOfChallenges = 0;
        float completedChallengeCount = 0;

        if (packLogs != null && packLogs.Length > 0 && completedChallenges != null && completedChallenges.Count > 0)
        {
            ChallengeLog currentLog;
            int currentChallengeCount;

            for (int index = 0; index < packLogs.Length; index++)
            {
                currentLog = packLogs[index];
                int levelIndex = Int32.Parse(currentLog.name.Split('_')[1]);

                currentChallengeCount = currentLog.GetChallengeCount();
                amountOfChallenges += currentChallengeCount;

                for (int challengeIndex = 0; challengeIndex < currentChallengeCount; challengeIndex++)
                {
                    int challengeKey = Challenge.GetChallengeKey(packIndex + 1, levelIndex, challengeIndex);
                    if (completedChallenges.TryGetValue(challengeKey, out bool result))
                        if (result == true)
                            completedChallengeCount++;
                }
            }

            completionText.text = $"{((completedChallengeCount / amountOfChallenges) * 100).ToString("F0")}%";
        }
        else
            completionText.text = "--%";
    }
}
