using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryDisplay : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameManager gameManager = null;
    [SerializeField] private UnlockManager unlockManager = null;
    [SerializeField] private ChallengePreview challengePreview = null;

    [Header("Component References")]
    [SerializeField] private Animator anim = null;

    private delegate void OnExitAnimationComplete();
    private event OnExitAnimationComplete onExitAnimationComplete = null;

    private void OnEnable()
    {
        UpdateChallengePreview();
    }

    public void DisplayNotifications()
    {
        if (unlockManager.CheckForCompletedUnlocks(gameManager.GetPackIndex(), gameManager.GetLevelIndex()))
            unlockManager.StartNotificationDisplay();
    }

    public void UpdateChallengePreview()
    {
        Vector2 indexes = LevelParser.GetLevelPackLevelIndexes(SceneManager.GetActiveScene().name);
        ChallengeLog challengeLog = ChallengeManager.GetCurrentChallengeLog((int)indexes.x, (int)indexes.y);
        challengePreview.SetLevelChallengePreview(challengeLog, SceneManager.GetActiveScene().name);
    }

    public void ExecuteOnAnimationFinish() { onExitAnimationComplete?.Invoke(); }
    public void NavigateToNextLevel()
    {
        anim.SetTrigger("Exit");
        onExitAnimationComplete += gameManager.NavigateToNextLevel;
    }

    public void RestartLevel()
    {
        anim.SetTrigger("Exit");
        onExitAnimationComplete += gameManager.RestartCurrentLevel;
    }

    public void ExitToMainMenu()
    {
        anim.SetTrigger("Exit");
        onExitAnimationComplete += gameManager.ExitToMainMenu;
    }
}
