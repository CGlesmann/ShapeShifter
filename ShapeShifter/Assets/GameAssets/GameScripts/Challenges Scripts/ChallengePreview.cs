using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengePreview : MonoBehaviour
{
    private ChallengeList currentLevelChallengeList;

    [Header("GUI References")]
    [SerializeField] private List<ChallengeEntry> challengeEntries = null;

    public void SetLevelChallengePreview(string levelName, int levelIndex)
    {
        ChallengeLog challengeLog = Resources.Load<ChallengeLog>("ChallengeLog/Challenge Log");
        if (challengeLog != null)
        {
            currentLevelChallengeList = challengeLog.GetLevelChallengeList(levelIndex);

            if (currentLevelChallengeList != null)
            {
                for (int i = 0; i < challengeEntries.Count; i++)
                {
                    if (currentLevelChallengeList.challenges.Count - 1 < i)
                        challengeEntries[i].DisableChallengeEntry();
                    else
                    {
                        challengeEntries[i].gameObject.SetActive(true);

                        Challenge challengeData = currentLevelChallengeList.challenges[i] as Challenge;
                        challengeEntries[i].UpdateChallengeText(challengeData.challengeDescription);

                        if (DataTracker.gameData.GetLevelChallengeResult(levelName, i))
                            challengeEntries[i].MarkAsCompleted();
                    }
                }
            }
        }
    }
}
