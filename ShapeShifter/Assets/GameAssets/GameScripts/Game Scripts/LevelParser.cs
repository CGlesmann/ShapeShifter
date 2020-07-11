using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelParser
{
    public static Vector2 GetLevelPackLevelIndexes(string levelName)
    {
        Vector2 indexes = new Vector2(GetLevelPackIndex(levelName), GetLevelIndex(levelName));
        return indexes;
    } 

    public static int GetLevelPackIndex(string levelName)
    {
        if (levelName.Contains("Level_"))
        {
            string[] indexes = LevelLoader.GetLevelName().Split('_')[1].Split('-');
            return Int32.Parse(indexes[0]);
        }

        return -1;
    }

    public static int GetLevelIndex(string levelName)
    {
        if (levelName.Contains("Level_"))
        {
            string[] indexes = LevelLoader.GetLevelName().Split('_')[1].Split('-');
            return Int32.Parse(indexes[1]);
        }

        return -1;
    }
}
