using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveDataViewer : EditorWindow
{
    [MenuItem(itemName: "Save Data Viewer", menuItem = "Window/Save Data Viewer")]
    public static void DisplayWindow()
    {
        CreateInstance<SaveDataViewer>().Show();
    }

    private void OnGUI()
    {
        if (DataTracker.gameData == null)
        {
            EditorGUILayout.LabelField("No Game Data is Available", EditorStyles.boldLabel, GUILayout.Width(300));
            return;
        }

        EditorGUILayout.LabelField("Highest Completed Levels (Per Packs)");
        foreach (KeyValuePair<int, int> pair in DataTracker.gameData.completedLevels)
        {
            EditorGUILayout.LabelField($"highestCompletedLevel: {pair.Value} ({pair.Key})");
            EditorGUILayout.Separator();
        }

        EditorGUILayout.LabelField(string.Format("highestDisplayedUnlock: {0}", DataTracker.gameData.highestLevelUnlocked));
        EditorGUILayout.Separator();
    }
}
