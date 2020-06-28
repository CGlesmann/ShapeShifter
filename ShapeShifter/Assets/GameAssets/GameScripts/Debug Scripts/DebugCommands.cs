using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
}
