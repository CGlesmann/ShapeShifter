﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveKeys
{
    public const string UNLOCK_SAVE_KEY = "UnlockDictionary";
    public const string COMPLETED_LEVELS_SAVE_KEY = "CompletedLevels";
    public const string COMPLETED_CHALLENGES_SAVE_KEY = "CompletedChallenges";
    public const string BEST_LEVEL_TIMES = "BestLevelTimes";

    public const string HIGHEST_DISPLAYED_PACK_UNLOCK = "HighestPackDisplay";
    public const string HIGHEST_DISPLAYED_LEVEL_UNLOCK = "HighestLevelDisplay";

    public const string INITIAL_TUTORIAL_COMPLETE = "InitialTutorialComplete";
    public const string DESTRUCT_TUTORIAL_COMPLETE = "DestructTutorialComplete";
    public const string LOCK_TUTORIAL_COMPLETE = "LockTutorialComplete";
    public const string DEFEAT_TUTORIAL_COMPLETE = "DefeatTutorialComplete";
    public const string TRANSFORMER_TUTORIAL_COMPLETE = "TransformerTutorialComplete";

    public const string SELECTED_COLOR_MODE = "SelectedColorMode";
    public const string SELECTED_THEME_KEY = "SelectedThemeKey";
}

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
            LoadData();

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
    private Dictionary<string, DataEntry> dataEntries = new Dictionary<string, DataEntry>();

    public void SetData(string key, object data)
    {
        if (dataEntries == null)
        {
            dataEntries = new Dictionary<string, DataEntry>();
            dataEntries.Add(key, new DataEntry(data));
        }
        else if (dataEntries.ContainsKey(key))
        {
            dataEntries[key].SetData(data);
        }
        else
            dataEntries.Add(key, new DataEntry(data));
    }

    public T GetDataValue<T>(string key)
    {
        if (dataEntries == null || !dataEntries.ContainsKey(key))
            return default;

        return dataEntries[key].GetData<T>();
    }
}

[System.Serializable]
public class DataEntry
{
    private object entryData = null;

    public DataEntry(object data) { SetData(data); }
    public void SetData(object data) { entryData = data; }

    public T GetData<T>()
    {
        if (entryData == null)
            return default;

        return (T)entryData;
    }
}