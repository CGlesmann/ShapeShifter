using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public static class StartOverride
{
    private const string textFilePath = "Editor Tools/ReloadData.json";
    private static string fullTextFilePath => "Assets/Editor Default Resources/" + textFilePath;

    private const string MAIN_MENU_PATH = "Assets/GameScenes/Menu Scenes/MainMenu.unity";

    [MenuItem("Start From Main Menu", menuItem = "Global Tools/Start Main Menu")]
    public static void StartFromMainMenu()
    {
        if (!EditorApplication.isPlaying)
        {
            StreamWriter writer = new StreamWriter(fullTextFilePath);
            writer.WriteLine(EditorSceneManager.GetActiveScene().path);
            writer.Close();

            AssetDatabase.ImportAsset(fullTextFilePath);

            EditorSceneManager.OpenScene(MAIN_MENU_PATH);
            EditorApplication.isPlaying = true;
        }
    }

    [InitializeOnLoadMethod]
    public static void SubscribeReloadFunction() { EditorApplication.playModeStateChanged += ReloadInitialScene;}
    public static void ReloadInitialScene(PlayModeStateChange mode)
    {
        if (mode == PlayModeStateChange.EnteredEditMode)
        {
            StreamReader reader = new StreamReader(fullTextFilePath);
            string sceneToLoad = reader.ReadLine();
            reader.Close();

            if (sceneToLoad != "null")
            {
                EditorSceneManager.OpenScene(sceneToLoad);
                StreamWriter writer = new StreamWriter(fullTextFilePath);
                writer.WriteLine("null");
                writer.Close();
            }
        }
    }
}
