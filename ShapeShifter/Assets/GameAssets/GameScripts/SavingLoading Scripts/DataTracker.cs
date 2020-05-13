using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataTracker : MonoBehaviour
{
    public static DataTracker dataTracker = null;
    public static GameData gameData = null;

    private string filePath => Application.persistentDataPath + "/playerData.dat";

    private void Awake()
    {
        if (dataTracker == null)
        {
            dataTracker = this;
            gameData = new GameData();

            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);

        bf.Serialize(file, gameData);
        file.Close();
    }

    public void LoadData()
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);

            gameData = (GameData)bf.Deserialize(file);
            file.Close();
        }
    }

    public void ResetSaveData()
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);

            gameData = new GameData();
            bf.Serialize(file, gameData);
            file.Close();
        }
    }
}

[System.Serializable]
public class GameData
{
    public bool initialTutorialComplete = false;
    public bool destroyTutorialComplete = false;
    public bool defeatTutorialComplete = false;
    public bool lockTutorialComplete = false;

    public int highestLevelUnlocked = 0;
    public int highestPackUnlocked = 0;

    public int starCount = 0;
    public Dictionary<int, int> completedLevels = new Dictionary<int, int>();
    public Dictionary<int, bool> completedChallenges = new Dictionary<int, bool>();

    public Dictionary<int, bool> displayedUnlocks = new Dictionary<int, bool>();

    public void MarkUnlockAsDisplayed(int index)
    {
        if (!displayedUnlocks.ContainsKey(index))
            displayedUnlocks.Add(index, true);
    }

    public bool GetUnlockDisplayed(int index)
    {
        if (displayedUnlocks.TryGetValue(index, out bool value))
            return value;

        return false;
    }

    public void MarkLevelComplete(int packIndex, int levelIndex)
    {
        if (completedLevels.ContainsKey(packIndex))
        {
            if (levelIndex > completedLevels[packIndex])
            {
                completedLevels[packIndex] = levelIndex;
            }
        }
        else
            completedLevels.Add(packIndex, levelIndex);
    }

    public void AddChallengeResult(int challengeKey, bool result)
    {
        if (completedChallenges.ContainsKey(challengeKey))
            completedChallenges[challengeKey] = result;
        else
            completedChallenges.Add(challengeKey, result);
    }

    public bool GetChallengeResult(int challengeKey)
    {
        if (completedChallenges.TryGetValue(challengeKey, out bool result))
            return result;

        return false;
    }
}