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
    private bool locked = false;

    private void OnEnable()
    {
        SetLockDisplay();
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

        ChallengeLog challengeLog = ChallengeManager.GetCurrentChallengeLog(manager.levelPackIndex + 1, buttonIndex + 1);
        if (challengeLog != null)
        {
            for(int i = 0; i < challengeLog.GetChallengeCount(); i++)
            {
                if (!DataTracker.gameData.GetChallengeResult(Challenge.GetChallengeKey(manager.levelPackIndex + 1, buttonIndex + 1, i)))
                    return;
            }

            completionIcon.SetActive(true);
        }
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
