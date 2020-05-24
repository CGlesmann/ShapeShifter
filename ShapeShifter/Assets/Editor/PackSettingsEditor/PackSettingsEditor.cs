using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PackSettingsEditor : EditorWindow
{
    private PackSettings currentPackSettings = null;
    private bool creatingSettingsAsset = false;

    private string packSettingsAssetPath => "PackSettings";

    private string assetToCreateName = "";

    [MenuItem("Global Tools", menuItem = "Global Tools/Pack Settings Editor")]
    public static void OpenEditor()
    {
        PackSettingsEditor editor = GetWindow<PackSettingsEditor>();
        editor.Show();
    }

    private void OnGUI()
    {
        #region Header Section
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Pack Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(150f));
        if (GUILayout.Button("Toggle Active Asset", GUILayout.MaxWidth(150f)))
        {
            GenericMenu actionMenu = new GenericMenu();
            actionMenu.AddItem(new GUIContent("Create New Asset"), false, BeginCreatingSettingsAsset);

            PackSettings[] createdAssets = Resources.LoadAll<PackSettings>(packSettingsAssetPath);
            if (createdAssets != null && createdAssets.Length > 0)
            {
                actionMenu.AddSeparator("");
                foreach(PackSettings asset in createdAssets)
                    actionMenu.AddItem(new GUIContent($"Select {asset.name}"), false, SetCurrentPackSettingsAsset, asset);
            }

            actionMenu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        EditorGUILayout.Separator();

        #region Panel Draw Calls
        if (creatingSettingsAsset)
            DrawCreationPanel();
        else if (currentPackSettings != null)
            DrawEditPanel();
        else
            EditorGUILayout.LabelField("Select A Pack Settings Asset To Begin Editing", EditorStyles.centeredGreyMiniLabel);
        #endregion
    }

    #region Helper Functions
    private void SetCurrentPackSettingsAsset(object asset) { currentPackSettings = (PackSettings)asset; }

    private void BeginCreatingSettingsAsset() { creatingSettingsAsset = true; }

    private void CreateSettingsAsset()
    {
        PackSettings newAsset = CreateInstance<PackSettings>();
        newAsset.name = assetToCreateName;

        if (!Directory.Exists($"Assets/Resources/{packSettingsAssetPath}"))
            AssetDatabase.CreateFolder("Assets/Resources", packSettingsAssetPath);

        AssetDatabase.CreateAsset(newAsset, $"Assets/Resources/{packSettingsAssetPath}/{assetToCreateName}.asset");
        AssetDatabase.Refresh();
    }
    #endregion

    #region Drawing Functions
    private void DrawCreationPanel()
    {
        GUIStyle headerStyle = new GUIStyle();
        headerStyle.fontSize = 20;
        headerStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.LabelField($"Create A Settings Asset", headerStyle);
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        assetToCreateName = EditorGUILayout.TextField("Asset Name", assetToCreateName, GUILayout.MaxWidth(350f));
        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Asset", GUILayout.MaxWidth(150f)))
        {
            CreateSettingsAsset();
            creatingSettingsAsset = false;
        }
        if (GUILayout.Button("Cancel", GUILayout.MaxWidth(150f)))
        {
            creatingSettingsAsset = false;
            return;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawEditPanel()
    {
        GUIStyle headerStyle = new GUIStyle();
        headerStyle.fontSize = 20;
        headerStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.LabelField($"Editing {currentPackSettings?.name.Replace('_', ' ')}", headerStyle);
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        currentPackSettings.levelCount = EditorGUILayout.IntField("Level Count", currentPackSettings.levelCount, GUILayout.MaxWidth(250f));
    }
    #endregion
}
