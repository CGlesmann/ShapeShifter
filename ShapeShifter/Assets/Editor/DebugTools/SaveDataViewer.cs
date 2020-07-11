using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveDataViewer : EditorWindow
{
    private SaveDataAccessor saveDataAccessor;
    private Vector2 scrollView;

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

        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        Dictionary<int, bool> completedChallenges = saveDataAccessor.GetDataValue<Dictionary<int, bool>>(SaveKeys.COMPLETED_CHALLENGES_SAVE_KEY);

        if (completedChallenges == null)
            return;

        scrollView = EditorGUILayout.BeginScrollView(scrollView);
        foreach (KeyValuePair<int, bool> entry in completedChallenges)
            EditorGUILayout.LabelField($"{entry.Key}: {entry.Value}");
        EditorGUILayout.EndScrollView();
    }
}
