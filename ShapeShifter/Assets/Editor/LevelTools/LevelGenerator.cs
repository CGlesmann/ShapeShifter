using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelGenerator : EditorWindow
{
    private int currentTab = 0;
        
    [MenuItem("Level Generator", menuItem = "Level Tools/Level Generator")]
    public static void DisplayGenerator()
    {
        LevelGenerator lgWindow = CreateInstance<LevelGenerator>();
        lgWindow.Show();
    }

    private void OnGUI()
    {
        // Drawing/Getting tab selection
        currentTab = GUILayout.Toolbar(currentTab, new string[] { "Generate Board", "Generate Solution"});
        
        switch(currentTab)
        {
            case 0:
                DrawBoardGenerator();
                break;
            case 1:
                DrawSolutionGenerator();
                break;
            default:
                EditorGUILayout.LabelField("currentTab set to invalid value: " + currentTab.ToString());
                break;
        }
    }

    private void DrawBoardGenerator()
    {
        EditorGUILayout.LabelField("This is the Board generator");
    }

    private void DrawSolutionGenerator()
    {
        EditorGUILayout.LabelField("This is the Solution generator");
    }
}
