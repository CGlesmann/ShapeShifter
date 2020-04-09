using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ChallengeManager : MonoBehaviour
{
    private ChallengeList currentLevelChallengeList;

    [Header("Object References")]
    [SerializeField] private GameManager manager = null;

    [Header("GUI References")]
    [SerializeField] private List<ChallengeEntry> challengeEntries = null;

    private void Awake()
    {
        ChallengeLog challengeLog = Resources.Load<ChallengeLog>("ChallengeLog/Challenge Log");
        if (challengeLog != null)
        {
            currentLevelChallengeList = challengeLog.GetLevelChallengeList(0);

            if (currentLevelChallengeList != null)
            {
                for (int i = 0; i < challengeEntries.Count; i++)
                {
                    if (!DataTracker.gameData.GetLevelChallengeResult(SceneManager.GetActiveScene().name, i) && i <= currentLevelChallengeList.challenges.Count - 1)
                        currentLevelChallengeList.challenges[i].SubscribeToCheck();
                }
            }
        }
    }

    public void UpdateChallengeEntries()
    {
        bool needToSave = false;

        for(int i = 0; i < challengeEntries.Count; i++)
        {
            if (currentLevelChallengeList == null || currentLevelChallengeList.challenges == null || i > currentLevelChallengeList.challenges.Count - 1)
                challengeEntries[i].DisableChallengeEntry();
            else
            {
                Challenge challenge = currentLevelChallengeList.challenges[i] as Challenge;
                challengeEntries[i].UpdateChallengeText(challenge.challengeDescription);

                if (currentLevelChallengeList.challenges[i].CheckForCompletedChallenge())
                {
                    challengeEntries[i].MarkAsCompleted();
                    DataTracker.gameData.MarkLevelChallengeComplete(SceneManager.GetActiveScene().name, i);

                    needToSave = true;
                }
            }
        }

        if (needToSave)
            DataTracker.dataTracker.SaveData();
    }
}
