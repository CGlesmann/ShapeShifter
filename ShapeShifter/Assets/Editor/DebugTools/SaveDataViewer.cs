using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveDataViewer : EditorWindow
{
    private SaveDataAccessor saveDataAccessor;

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

        /*
        if (saveDataAccessor == null)
            saveDataAccessor = new SaveDataAccessor();

        int highestDisplayedUnlock = saveDataAccessor.GetDataValue<int>(SaveKeys.HIGHEST_DISPLAYED_LEVEL_UNLOCK);
        EditorGUILayout.LabelField($"Highest Displayed Unlock: {highestDisplayedUnlock}");

        EditorGUILayout.LabelField("Completed Levels");
        Dictionary<int, int> completedLevels = saveDataAccessor.GetDataValue<Dictionary<int, int>>(SaveKeys.COMPLETED_LEVELS_SAVE_KEY);
        if (completedLevels == null || completedLevels.Count == 0)
            EditorGUILayout.LabelField("No levels have been completed");
        else
        {
            foreach(KeyValuePair<int, int> packLevelsCompleted in completedLevels)
                EditorGUILayout.LabelField($"Highest Completed Level for Pack{packLevelsCompleted.Key}: {packLevelsCompleted.Value}");
        }
        */
    }
}
