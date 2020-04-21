using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SettingsEditor : EditorWindow
{
    private static ShapeSettingsConfig configFile = null;

    [MenuItem("Shape Settings", menuItem = "Settings/Shape Settings")]
    public static void DisplayWindow()
    {
        SettingsEditor window = (SettingsEditor)CreateInstance(typeof(SettingsEditor));
        window.Show();
    }

    private void OnGUI()
    {
        if (configFile == null)
            configFile = Resources.Load<ShapeSettingsConfig>("Settings/ShapeSettingsConfig");

        #region Color Fields
        EditorGUILayout.LabelField("Shape Colors", EditorStyles.boldLabel, GUILayout.MaxWidth(150f));
        EditorGUILayout.Separator();

        string[] colorTags = Enum.GetNames(typeof(GameShape.ColorType));
        GameShape.ColorType currentColorType;

        for(int c = 0; c < colorTags.Length; c++)
        {
            currentColorType = (GameShape.ColorType)c;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{colorTags[c]}:", GUILayout.MaxWidth(150f));
            configFile.shapeColors.SetValue(currentColorType, EditorGUILayout.ColorField(configFile.shapeColors.GetValue(currentColorType), GUILayout.MaxWidth(150f)));
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        EditorGUILayout.Separator();

        #region Sprite Fields
        EditorGUILayout.LabelField("Shape Colors", EditorStyles.boldLabel, GUILayout.MaxWidth(150f));
        EditorGUILayout.Separator();

        string[] shapeTags = Enum.GetNames(typeof(GameShape.ShapeType));
        GameShape.ShapeType currentShapeType;

        for (int s = 0; s < colorTags.Length; s++)
        {
            currentShapeType = (GameShape.ShapeType)s;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{shapeTags[s]}:", GUILayout.MaxWidth(150f));
            configFile.shapeSprites.SetValue(currentShapeType, (Sprite)EditorGUILayout.ObjectField(configFile.shapeSprites.GetValue(currentShapeType), typeof(Sprite), false, GUILayout.MaxWidth(150f)));
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        EditorUtility.SetDirty(configFile);
    }
}
