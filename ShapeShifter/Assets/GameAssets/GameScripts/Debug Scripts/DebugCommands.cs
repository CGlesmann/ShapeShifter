using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DebugCommands
{
    [Command]
    public static void SetHighestPackLevel(int packIndex, int levelIndex)
    {
        if (packIndex < 1 || levelIndex < 1)
        {
            Debug.LogError("Indexes Out Of Range, Try again (Indexes start from 1)");
            return;
        }

        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();

        #region Save Completed Level Index
        Dictionary<int, int> completedLevels = saveDataAccessor.GetDataValue<Dictionary<int, int>>(SaveKeys.COMPLETED_LEVELS_SAVE_KEY);

        if (completedLevels == null)
        {
            completedLevels = new Dictionary<int, int>();
            completedLevels.Add(packIndex, levelIndex);

            saveDataAccessor.SetData(SaveKeys.COMPLETED_LEVELS_SAVE_KEY, completedLevels);
            DataTracker.dataTracker.SaveData();
        }
        else if (!completedLevels.ContainsKey(packIndex))
        {
            completedLevels.Add(packIndex, levelIndex);

            saveDataAccessor.SetData(SaveKeys.COMPLETED_LEVELS_SAVE_KEY, completedLevels);
            DataTracker.dataTracker.SaveData();
        }
        else
        {
            int highestCompletedLevel = completedLevels[packIndex];
            if (levelIndex > highestCompletedLevel)
            {
                completedLevels[packIndex] = levelIndex;
                saveDataAccessor.SetData(SaveKeys.COMPLETED_LEVELS_SAVE_KEY, completedLevels);
                DataTracker.dataTracker.SaveData();
            }
        }
        #endregion
    }

    [Command]
    public static void UnlockAllLevels()
    {
        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();

        #region Save Completed Level Index
        Dictionary<int, int> completedLevels = saveDataAccessor.GetDataValue<Dictionary<int, int>>(SaveKeys.COMPLETED_LEVELS_SAVE_KEY);
        int highestLevelIndex = 99;

        for (int packIndex = 1; packIndex <= 3; packIndex++)
        {
            if (completedLevels == null)
            {
                completedLevels = new Dictionary<int, int>();
                completedLevels.Add(packIndex, highestLevelIndex);

                saveDataAccessor.SetData(SaveKeys.COMPLETED_LEVELS_SAVE_KEY, completedLevels);
                DataTracker.dataTracker.SaveData();
            }
            else if (!completedLevels.ContainsKey(packIndex))
            {
                completedLevels.Add(packIndex, highestLevelIndex);

                saveDataAccessor.SetData(SaveKeys.COMPLETED_LEVELS_SAVE_KEY, completedLevels);
                DataTracker.dataTracker.SaveData();
            }
            else
            {
                int highestCompletedLevel = completedLevels[packIndex];
                if (highestLevelIndex > highestCompletedLevel)
                {
                    completedLevels[packIndex] = highestLevelIndex;
                    saveDataAccessor.SetData(SaveKeys.COMPLETED_LEVELS_SAVE_KEY, completedLevels);
                    DataTracker.dataTracker.SaveData();
                }
            }
        }
        #endregion
    }

    [Command]
    public static void ClearChallenges()
    {
        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        saveDataAccessor.SetData(SaveKeys.COMPLETED_CHALLENGES_SAVE_KEY, null);
        DataTracker.dataTracker.SaveData();
    }

    [Command]
    public static void SetAllChallengesToComplete()
    {
        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        Dictionary<int, bool> completedChallenges = saveDataAccessor.GetDataValue<Dictionary<int, bool>>(SaveKeys.COMPLETED_CHALLENGES_SAVE_KEY);
        if (completedChallenges == null)
            completedChallenges = new Dictionary<int, bool>();

        string baseChallengeLogs = "Assets/Resources/ChallengeLogs/";

        ChallengeLog[] packLogs;
        string[] levelPackDirectories = Directory.GetDirectories(baseChallengeLogs);

        for(int i = 0; i < levelPackDirectories.Length; i++)
        {
            packLogs = Resources.LoadAll<ChallengeLog>($"ChallengeLogs/Level_Pack_{i + 1}/");
            if (packLogs != null)
            {
                ChallengeLog currentLog;
                for(int j = 0; j < packLogs.Length; j++)
                {
                    currentLog = packLogs[j];
                    Debug.Log($"{currentLog.name} has {currentLog.GetChallengeCount()} challenges");
                    for(int h = 0; h < currentLog.GetChallengeCount(); h++)
                    {
                        int levelIndex = Int32.Parse(currentLog.name.Split('_')[1]);
                        Debug.Log($"Setting Challenge {h + 1} for Level_{i + 1}-{levelIndex}");
                        int key = Challenge.GetChallengeKey(i + 1, levelIndex, h);
                        if (completedChallenges.TryGetValue(key, out bool val))
                            completedChallenges[key] = true;
                        else
                            completedChallenges.Add(key, true);
                    }
                } 
            }
        }

        saveDataAccessor.SetData(SaveKeys.COMPLETED_CHALLENGES_SAVE_KEY, completedChallenges);
        DataTracker.dataTracker.SaveData();
    }
}
