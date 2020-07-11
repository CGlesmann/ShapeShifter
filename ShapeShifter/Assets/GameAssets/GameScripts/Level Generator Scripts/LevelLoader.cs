using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LevelLoader
{
    private static LevelData levelDataToLoad;
    private static string levelNameLoaded;

    public static bool SetLevelToLoad(string levelName) 
    {
        string indexes = levelName.Split('_')[1];
        int packIndex = Int32.Parse(indexes.Split('-')[0]);
        int levelIndex = Int32.Parse(indexes.Split('-')[1]);

        string path = $"LevelFiles/LevelPack_{packIndex}/Level_{packIndex}-{levelIndex}";
        TextAsset levelTextAsset = Resources.Load<TextAsset>(path);

        if (levelTextAsset != null)
        {
            StreamReader streamReader = new StreamReader($"Assets/Resources/LevelFiles/LevelPack_{packIndex}/Level_{packIndex}-{levelIndex}.json");
            string levelData = streamReader.ReadToEnd();

            levelNameLoaded = levelName;
            levelDataToLoad = JsonUtility.FromJson<LevelData>(levelData);

            return true;
        }
        else
        {
            levelDataToLoad = null;
            levelNameLoaded = "";

            return false;
        }
    }

    public static void SetLevelToLoad(LevelData levelData) { levelDataToLoad = levelData; }
    public static LevelData GetLevelToLoad() { return levelDataToLoad; }
    public static string GetLevelName() { return levelNameLoaded; }
}
