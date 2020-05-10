using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameManager gameManager = null;
    private ChallengeLog currentChallengeLog = null;

    private int packIndex;
    private int levelIndex;

    private void Start()
    {
        gameManager.onVictory += CheckChallengesForCompletion;
        packIndex = gameManager.GetPackIndex();
        levelIndex = gameManager.GetLevelIndex();

        SubscribeChallenges();
    }

    private void SubscribeChallenges()
    {
        if (currentChallengeLog == null)
        {
            currentChallengeLog = GetCurrentChallengeLog(packIndex, levelIndex);
            if (currentChallengeLog == null)
                return;
        }

        int challengeCount = currentChallengeLog.GetChallengeCount();
        for (int i = 0; i < challengeCount; i++)
        {
            if (!DataTracker.gameData.GetChallengeResult(Challenge.GetChallengeKey(packIndex, levelIndex, i)))
            {
                IChallenge challenge = currentChallengeLog.GetChallengeData(i) as IChallenge;
                challenge.SetUpChallenge();
            }
        }
    }

    public static ChallengeLog GetCurrentChallengeLog(int packIndex, int levelIndex)
    {
        string challengeLogPath = $"ChallengeLogs/Level_Pack_{packIndex}/Level_{levelIndex}";
        return Resources.Load<ChallengeLog>(challengeLogPath);
    }

    public void CheckChallengesForCompletion()
    {
        ChallengeLog challengeLog = GetCurrentChallengeLog(packIndex, levelIndex);
        if (challengeLog != null)
        {
            int challengeCount = challengeLog.GetChallengeCount();
            for(int i = 0; i < challengeCount; i++)
            {
                IChallenge challenge = challengeLog.GetChallengeData(i) as IChallenge;
                if (challenge != null)
                {
                    int challengeKey = Challenge.GetChallengeKey(packIndex, levelIndex, i);
                    if (challenge.CheckForCompletedChallenge() && !DataTracker.gameData.GetChallengeResult(challengeKey))
                        DataTracker.gameData.AddChallengeResult(challengeKey, true);
                }
            }
        }
    }
}
