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

    public int highestCompletedLevel = 0;
    public int highestLevelUnlocked = 0;
    public int highestPackUnlocked = 0;

    public Dictionary<string, List<bool>> levelChallenges = new Dictionary<string, List<bool>>();

    public void MarkLevelChallengeComplete(string key, int targetIndex)
    {
        if (levelChallenges.TryGetValue(key, out List<bool> challengeToggleList))
        {
            if (challengeToggleList == null)
            {
                challengeToggleList = new List<bool>();
                for (int i = 0; i < 3; i++)
                    challengeToggleList.Add(false);
            }

            challengeToggleList[targetIndex] = true;
        } else
        {
            challengeToggleList = new List<bool>();
            for (int i = 0; i < 3; i++)
                challengeToggleList.Add(false);

            challengeToggleList[targetIndex] = true;
            levelChallenges.Add(key, challengeToggleList);
        }
    }

    public bool GetLevelChallengeResult(string key, int challengeIndex)
    {
        if (levelChallenges.TryGetValue(key, out List<bool> challengeToggleList))
        {
            if (challengeToggleList == null || challengeIndex > challengeToggleList.Count - 1)
                return false;

            return challengeToggleList[challengeIndex];
        }
        else
            return false;
    }
}