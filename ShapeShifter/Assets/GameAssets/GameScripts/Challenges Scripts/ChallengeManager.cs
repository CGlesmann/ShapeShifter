using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameManager gameManager = null;
    private ChallengeLog currentChallengeLog = null;

    private List<IChallenge> currentChallenges;
    private int packIndex;
    private int levelIndex;

    private void OnEnable()
    {
        SubscribeChallenges(gameManager.GetPackIndex(), gameManager.GetLevelIndex());

        gameManager.onVictory += CheckChallengesForCompletion;
        gameManager.onVictory += UnsubscribeCurrentChallenges;
        gameManager.onLevelSet += SubscribeChallenges;
    }

    private void OnDisable()
    {
        gameManager.onVictory -= CheckChallengesForCompletion;
        gameManager.onVictory -= UnsubscribeCurrentChallenges;
        gameManager.onLevelSet -= SubscribeChallenges;
    }

    private void SubscribeChallenges(int pack, int level)
    {
        packIndex = pack;
        levelIndex = level;
        currentChallengeLog = GetCurrentChallengeLog(packIndex, levelIndex);

        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        Dictionary<int, bool> challengeDictionary = saveDataAccessor.GetDataValue<Dictionary<int, bool>>(SaveKeys.COMPLETED_CHALLENGES_SAVE_KEY);
        
        int challengeCount = currentChallengeLog.GetChallengeCount();
        currentChallenges = new List<IChallenge>();

        for (int i = 0; i < challengeCount; i++)
        {
            int challengeKey = Challenge.GetChallengeKey(packIndex, levelIndex, i);
            if (challengeDictionary == null || !challengeDictionary.ContainsKey(challengeKey) || challengeDictionary[challengeKey] == false)
            {
                IChallenge challenge = currentChallengeLog.GetChallengeData(i) as IChallenge;
                challenge.SetUpChallenge();

                currentChallenges.Add(challenge);
            }
        }
    }

    public void UnsubscribeCurrentChallenges()
    {
        for(int i = 0; i < currentChallenges.Count; i++)
            currentChallenges[i].UnsubscribeChallenge();
    }

    public static ChallengeLog GetCurrentChallengeLog(int packIndex, int levelIndex)
    {
        string challengeLogPath = $"ChallengeLogs/Level_Pack_{packIndex}/Level_{levelIndex}";
        return Resources.Load<ChallengeLog>(challengeLogPath);
    }

    public void CheckChallengesForCompletion()
    {
        if (currentChallengeLog != null)
        {
            SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
            Dictionary<int, bool> challengeDictionary = saveDataAccessor.GetDataValue<Dictionary<int, bool>>(SaveKeys.COMPLETED_CHALLENGES_SAVE_KEY);
            int challengeCount = currentChallengeLog.GetChallengeCount();
            for(int i = 0; i < challengeCount; i++)
            {
                IChallenge challenge = currentChallengeLog.GetChallengeData(i) as IChallenge;
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
