using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            currentLevelChallengeList = challengeLog.GetLevelChallengeList(manager.levelIndex - 1);

            if (currentLevelChallengeList != null)
            {
                foreach (IChallenge challenge in currentLevelChallengeList.challenges)
                    challenge.SubscribeToCheck();
            }
        }
    }

    public void UpdateChallengeEntries()
    {
        for(int i = 0; i < challengeEntries.Count; i++)
        {
            if (currentLevelChallengeList == null || currentLevelChallengeList.challenges == null || i > currentLevelChallengeList.challenges.Count - 1)
                challengeEntries[i].DisableChallengeEntry();
            else
            {
                Challenge challenge = currentLevelChallengeList.challenges[i] as Challenge;
                challengeEntries[i].UpdateChallengeText(challenge.challengeDescription);

                if (currentLevelChallengeList.challenges[i].CheckForCompletedChallenge())
                    challengeEntries[i].MarkAsCompleted();
            }
        }
    }
}
