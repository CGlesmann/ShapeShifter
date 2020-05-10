using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private Animator anim = null;
    [SerializeField] private ChallengePreview challengePreview = null;

    private ChallengeLog challengeLog = null;

    private void OnEnable()
    {
        if (challengeLog == null)
        {
            Vector2 indexes = LevelParser.GetLevelPackLevelIndexes(SceneManager.GetActiveScene().name);
            challengeLog = ChallengeManager.GetCurrentChallengeLog((int)indexes.x, (int)indexes.y);
        }

        challengePreview.SetLevelChallengePreview(challengeLog, SceneManager.GetActiveScene().name);
    }

    public void DisablePanel() { gameObject.SetActive(false); }
    public void ExecuteExitTransition() { anim.SetTrigger("Exit"); }
}
