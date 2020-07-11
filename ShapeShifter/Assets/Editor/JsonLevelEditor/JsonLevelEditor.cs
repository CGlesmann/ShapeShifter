using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Content;

public class JsonLevelEditor : EditorWindow
{
    private const string LEVEL_FILES_LOCATION = "LevelFiles";

    private LevelData currentLevelData;
    private int currentPackIndex = -1;
    private int currentLevelIndex = -1;

    [MenuItem("Json Level Editor", menuItem = "Global Tools/Json Level Editor")]
    public static void OpenEditor()
    {
        JsonLevelEditor editorWindow = GetWindow<JsonLevelEditor>();
        editorWindow.Show();
    }

    private void OnGUI()
    {
        if (GameObject.FindObjectOfType<GameManager>() == null)
        {
            EditorGUILayout.HelpBox(new GUIContent("Navigate to a level scene to use this tool"));
            return;
        }

        GUIStyle headerStyle = new GUIStyle();
        headerStyle.fontSize = 18;
        headerStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.LabelField("Json Level Editor", headerStyle);
        EditorGUILayout.Separator();

        currentPackIndex = EditorGUILayout.IntField("Target Pack Index", currentPackIndex, GUILayout.MaxWidth(250f));
        currentLevelIndex = EditorGUILayout.IntField("Target Level Index", currentLevelIndex, GUILayout.MaxWidth(250f));
        
        if (GUILayout.Button("Create Level Asset"))
        {
            GameManager currentGameManager = GameObject.FindObjectOfType<GameManager>();
            BoardManager currentBoardManager = GameObject.FindObjectOfType<BoardManager>();
            BoardData newGameBoardData = new BoardData(currentGameManager.gameBoardParent, 
                                                       new Vector2Int(currentBoardManager.GetBoardWidth(), 
                                                                      currentBoardManager.GetBoardHeight()));
            BoardData newSolutionBoardData = new BoardData(currentGameManager.solutionBoardParent,
                                                       new Vector2Int(currentBoardManager.GetBoardWidth(),
                                                                      currentBoardManager.GetBoardHeight()));

            string targetFilePath = GetCurrentAssetPath();
            if (!File.Exists(targetFilePath))
            {
                AssetDatabase.CreateAsset(new TextAsset(), targetFilePath);
                AssetDatabase.Refresh();
            }

            LevelData targetLevelData = new LevelData(newGameBoardData, newSolutionBoardData);
            string levelJsonData = JsonUtility.ToJson(targetLevelData, true);
            StreamWriter writer = new StreamWriter(targetFilePath);

            writer.Write(levelJsonData);
            writer.Close();
        }

        if (GUILayout.Button("Load Level Data"))
        {
            GameObject.FindObjectOfType<LevelConstructor>().DestroyLevel();

            TextAsset levelDataAsset = Resources.Load<TextAsset>($"{LEVEL_FILES_LOCATION}/LevelPack_{currentPackIndex}/Level_{currentPackIndex}-{currentLevelIndex}");
            if (levelDataAsset == null)
            {
                Debug.LogError($"Couldn't Find Data Asset for {GetCurrentAssetPath()}");
                return;
            }

            StreamReader reader = new StreamReader(GetCurrentAssetPath());
            string input = reader.ReadToEnd();

            currentLevelData = JsonUtility.FromJson<LevelData>(input);
            if (currentLevelData == null)
            {
                Debug.LogError($"Couldn't Parse Level Data Asset");
                return;
            }

            LevelConstructor constructor = GameObject.FindObjectOfType<LevelConstructor>();
            constructor.ConstructLevel(currentLevelData);
        }

        if (GUILayout.Button("Destroy Level"))
        {
            GameObject.FindObjectOfType<LevelConstructor>().DestroyLevel();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }

    private string GetCurrentAssetPath() { return $"Assets/Resources/{LEVEL_FILES_LOCATION}/LevelPack_{currentPackIndex}/Level_{currentPackIndex}-{currentLevelIndex}.json"; }
}
