using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private GameManager gameManager = null;
    [SerializeField] private Animator anim = null;
    [SerializeField] private ChallengePreview challengePreview = null;

    private ChallengeLog challengeLog = null;

    private void OnEnable()
    {
        SetChallengeLog();
        challengePreview.SetLevelChallengePreview(challengeLog, LevelLoader.GetLevelName());
    }

    public void SetChallengeLog()
    {
        Vector2 indexes = LevelParser.GetLevelPackLevelIndexes(LevelLoader.GetLevelName());
        challengeLog = ChallengeManager.GetCurrentChallengeLog((int)indexes.x, (int)indexes.y);
    }

    public void DisablePanel() { gameObject.SetActive(false); }
    public void ExecuteExitTransition() { anim.SetTrigger("Exit"); }
}
