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

        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        Dictionary<int, bool> challengeDictionary = saveDataAccessor.GetDataValue<Dictionary<int, bool>>(SaveKeys.COMPLETED_CHALLENGES_SAVE_KEY);
        int challengeCount = currentChallengeLog.GetChallengeCount();

        for (int i = 0; i < challengeCount; i++)
        {
            int challengeKey = Challenge.GetChallengeKey(packIndex, levelIndex, i);
            if (challengeDictionary == null || !challengeDictionary.ContainsKey(challengeKey) || challengeDictionary[challengeKey] == false)
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
            SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
            Dictionary<int, bool> challengeDictionary = saveDataAccessor.GetDataValue<Dictionary<int, bool>>(SaveKeys.COMPLETED_CHALLENGES_SAVE_KEY);
            int challengeCount = challengeLog.GetChallengeCount();
            for(int i = 0; i < challengeCount; i++)
            {
                IChallenge challenge = challengeLog.GetChallengeData(i) as IChallenge;
                if (challenge != null)
                {
                    int challengeKey = Challenge.GetChallengeKey(packIndex, levelIndex, i);
                    if (challenge.CheckForCompletedChallenge())
                    {
                        if (challengeDictionary == null)
                        {
                            challengeDictionary = new Dictionary<int, bool>();
                            challengeDictionary.Add(challengeKey, true);

                            saveDataAccessor.SetData(SaveKeys.COMPLETED_CHALLENGES_SAVE_KEY, challengeDictionary);
                            DataTracker.dataTracker.SaveData();
                        }
                        else if (!challengeDictionary.ContainsKey(challengeKey))
                        {
                            challengeDictionary.Add(challengeKey, true);

                            saveDataAccessor.SetData(SaveKeys.COMPLETED_CHALLENGES_SAVE_KEY, challengeDictionary);
                            DataTracker.dataTracker.SaveData();
                        }
                        else
                            challengeDictionary[challengeKey] = true;
                    }
                }
            }
        }
    }
}
