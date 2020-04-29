using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LevelBuilder : EditorWindow
{
    private GameManager gameManager = null;
    private BoardManager boardManager = null;

    private readonly string[] tabTitles = { "Gameboard Tools", "GameSlot Tools", "Level Generator" };
    private int tabIndex = 0;
    private bool viewingGameboardSection = false;
    private bool viewingSolutionboardSection = false;

    private float sizing = 0f;

    private SlotLock.LockType desiredLockType = SlotLock.LockType.Destruct;
    private int desiredLockTimer = 1;
    private float desiredLockSize = 0f;

    private GameShape.ShapeType desiredShapeType = GameShape.ShapeType.Circle;
    private GameShape.ColorType desiredShapeColor = GameShape.ColorType.Blue;
    private float desiredShapeSize = 0f;

    private GeneratorShapePrefs genShapePrefs = new GeneratorShapePrefs();
    private bool genPrefsMismatchError = false;
    private bool zeroShapeScaleError = false;

    [MenuItem("Level Builder", menuItem = "Level Tools/Level Builder %&L")]
    public static void DisplayLevelBuilder()
    {
        LevelBuilder builderWindow = GetWindow<LevelBuilder>();
        builderWindow.Show();
    }

    private void OnGUI()
    {
        if (boardManager == null)
        {
            GetManagerReference();
            if (boardManager == null)
            {
                EditorGUILayout.LabelField("Navigate to a level to use the Level Builder");
                return;
            }
        }

        tabIndex = GUILayout.Toolbar(tabIndex, tabTitles, GUILayout.MaxWidth(500f));
        switch(tabIndex)
        {
            case 0:
                DrawGameboardTools();
                break;
            case 1:
                DrawGameSlotTools();
                break;
            case 2:
                DrawLevelGenerator();
                break;
        }
    }

    private void GetManagerReference() { boardManager = GameObject.Find("LevelManager")?.GetComponent<BoardManager>(); }
    private int GetBoardSize()
    {
        if (boardManager == null)
            GetManagerReference();

        return boardManager.GetBoardSize();
    }

    private void DrawGameboardTools()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Viewing Options", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Toggle Locks", GUILayout.MaxWidth(150f)))
        {
            GameboardTools.ToggleLocks();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Board Manipulation", EditorStyles.boldLabel);

        if (GUILayout.Button("Clear All Boards", GUILayout.MaxWidth(150f)))
        {
            GameboardTools.ClearGameboard(true, true);
            GameboardTools.ClearSolutionboard(true, true);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        if (GUILayout.Button("Reset All Slot References", GUILayout.MaxWidth(150f)))
        {
            if (gameManager == null)
                gameManager = GameObject.FindObjectOfType<GameManager>();

            GameboardTools.SetAllGameSlotReferences(gameManager.gameBoardParent);
            GameboardTools.SetAllGameSlotReferences(gameManager.solutionBoardParent);
        }

        #region Gameboard Manipulation
        viewingGameboardSection = EditorGUILayout.Foldout(viewingGameboardSection, "GameBoard Tools");

        if (viewingGameboardSection)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Resize GameShapes", GUILayout.MaxWidth(150f)))
            {
                GameboardTools.ResizeGameShapes(sizing != 0 ? sizing : boardManager.shapeSize);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            sizing = EditorGUILayout.FloatField(sizing, GUILayout.MaxWidth(50f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            if (GUILayout.Button("Reset Slot References", GUILayout.MaxWidth(150f)))
            {
                if (gameManager == null)
                    gameManager = GameObject.FindObjectOfType<GameManager>();

                GameboardTools.SetAllGameSlotReferences(gameManager.gameBoardParent);
            }

            EditorGUILayout.Separator();
            if (GUILayout.Button("Clear GameBoard", GUILayout.MaxWidth(150f)))
            {
                GameboardTools.ClearGameboard(true, true);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Shapes", GUILayout.MaxWidth(150f)))
            {
                GameboardTools.ClearGameboard(true, false);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            if (GUILayout.Button("Clear Locks", GUILayout.MaxWidth(150f)))
            {
                GameboardTools.ClearGameboard(false, true);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Solutionboard Manipulation
        viewingSolutionboardSection = EditorGUILayout.Foldout(viewingSolutionboardSection, "SolutionBoard Tools");

        if (viewingSolutionboardSection)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Resize GameShapes", GUILayout.MaxWidth(150f)))
            {
                GameboardTools.ResizeGameShapes(sizing != 0 ? sizing : boardManager.shapeSize);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            sizing = EditorGUILayout.FloatField(sizing, GUILayout.MaxWidth(50f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            if (GUILayout.Button("Reset Slot References", GUILayout.MaxWidth(150f)))
            {
                if (gameManager == null)
                    gameManager = GameObject.FindObjectOfType<GameManager>();

                GameboardTools.SetAllGameSlotReferences(gameManager.solutionBoardParent);
            }

            EditorGUILayout.Separator();
            if (GUILayout.Button("Clear SolutionBoard", GUILayout.MaxWidth(150f)))
            {
                GameboardTools.ClearSolutionboard(true, true);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Shapes", GUILayout.MaxWidth(150f)))
            {
                GameboardTools.ClearGameboard(true, false);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            if (GUILayout.Button("Clear Locks", GUILayout.MaxWidth(150f)))
            {
                GameboardTools.ClearGameboard(false, true);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion
    }

    private void DrawGameSlotTools()
    {
        EditorGUILayout.Separator();

        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects != null)
        {
            if (selectedObjects.Length > 1)
                EditorGUILayout.LabelField("Muli Select Editing isn't supported", EditorStyles.helpBox);
            else if (selectedObjects.Length == 1)
            {
                GameSlot currentSelectSlot = selectedObjects[0].GetComponent<GameSlot>();
                if (currentSelectSlot == null)
                    EditorGUILayout.LabelField("Please Select a GameSlot object to begin editing", EditorStyles.helpBox);
                else
                    DrawGameShapeOptions(currentSelectSlot);
            } else {
                EditorGUILayout.LabelField("Please Select a GameSlot object to begin editing", EditorStyles.helpBox);
            }
        }
    }
    private void DrawGameShapeOptions(GameSlot slot)
    {
        GameShape shapeData = slot.GetSlotShape();

        if (shapeData != null)
        {
            EditorGUILayout.LabelField($"Shape Info", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Shape Color: {shapeData.colorType}");
            EditorGUILayout.LabelField($"Shape Type: {shapeData.GetShapeType()}");
        }
        else
            EditorGUILayout.LabelField("Shape Info: No Shape Available");

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Lock Options", EditorStyles.boldLabel);

        desiredLockType = (SlotLock.LockType)EditorGUILayout.EnumPopup("Lock Type", desiredLockType);
        desiredLockTimer = EditorGUILayout.IntField("Lock Timer", desiredLockTimer);
        desiredLockSize = EditorGUILayout.FloatField("Lock Size", desiredLockSize);

        EditorGUILayout.BeginHorizontal();
        if (slot.GetSlotLock() != null)
        {
            if (GUILayout.Button("Edit Lock", GUILayout.MaxWidth(150f)))
            {
                GameSlotTools.EditSlotLock(slot.transform, desiredLockType, desiredLockTimer, desiredLockSize);
                EditorUtility.SetDirty(slot.GetComponent<GameSlot>().GetSlotLock());
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
        else
        {
            if (GUILayout.Button("Create Lock", GUILayout.MaxWidth(150f)))
            {
                GameSlotTools.CreateSlotLock(slot.transform, desiredLockType, desiredLockTimer, desiredLockSize);
                EditorUtility.SetDirty(slot.GetComponent<GameSlot>().GetSlotLock());
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
        
        SlotLock slotLock = slot.GetSlotLock();
        if (slotLock != null && GUILayout.Button("Destroy Lock", GUILayout.Width(150f)))
        {
            DestroyImmediate(slotLock.gameObject);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Shape Options", EditorStyles.boldLabel);

        desiredShapeType = (GameShape.ShapeType)EditorGUILayout.EnumPopup("Shape Type", desiredShapeType);
        desiredShapeColor = (GameShape.ColorType)EditorGUILayout.EnumPopup("Shape Color", desiredShapeColor);
        desiredShapeSize = EditorGUILayout.FloatField("Shape Scale", desiredShapeSize);

        EditorGUILayout.BeginHorizontal();
        if (slot.GetSlotShape() != null)
        {
            if (GUILayout.Button("Edit Shape", GUILayout.MaxWidth(150f)))
            {
                GameSlotTools.EditSlotShape(slot.transform, desiredShapeType, desiredShapeColor, desiredShapeSize);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
        else
        {
            if (GUILayout.Button("Create Shape", GUILayout.MaxWidth(150f)))
            {
                GameSlotTools.CreateSlotShape(slot.transform, desiredShapeType, desiredShapeColor, desiredShapeSize);
                EditorUtility.SetDirty(slot.GetComponent<GameSlot>());
                EditorUtility.SetDirty(slot.GetComponent<GameSlot>().GetSlotShape());
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        if (shapeData != null && GUILayout.Button("Destroy Shape", GUILayout.Width(150f)))
        {
            DestroyImmediate(shapeData.gameObject);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
    }

    private void DrawLevelGenerator()
    {
        #region General Board Settings
        EditorGUILayout.LabelField("Board Settings", EditorStyles.boldLabel);
        EditorGUILayout.Separator();

        genShapePrefs.targetShapeScale = EditorGUILayout.FloatField("Shape Scale", genShapePrefs.targetShapeScale, GUILayout.MaxWidth(200f));
        #endregion
        EditorGUILayout.Separator();

        #region Shape Settings
        EditorGUILayout.LabelField("Shape Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(150f));
        EditorGUILayout.Separator();

        string[] shapeTags = Enum.GetNames(typeof(GameShape.ShapeType));
        GameShape.ShapeType currentShapeType;
        for (int i = 0; i < shapeTags.Length; i++)
        {
            currentShapeType = (GameShape.ShapeType)i;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(20f), GUILayout.Height(20f)))
            {
                if (genShapePrefs.shapes.TryGetValue(currentShapeType, out int sCount))
                    genShapePrefs.shapes[currentShapeType] = Mathf.Clamp(sCount + 1, 0, sCount + genShapePrefs.GetRemainingShapes(GetBoardSize()));
                else
                    genShapePrefs.shapes.Add(currentShapeType, Mathf.Clamp(1, 0, sCount + genShapePrefs.GetRemainingShapes(GetBoardSize())));
            }

            if (GUILayout.Button("-", GUILayout.Width(20f), GUILayout.Height(20f)))
            {
                if (genShapePrefs.shapes.TryGetValue(currentShapeType, out int sCount))
                    genShapePrefs.shapes[currentShapeType] = Mathf.Clamp(sCount - 1, 0, sCount + genShapePrefs.GetRemainingShapes(GetBoardSize()));
                else
                    genShapePrefs.shapes.Add(currentShapeType, 0);
            }

            int squareCount = genShapePrefs.shapes.TryGetValue(currentShapeType, out int squares) ? squares : 0;
            EditorGUILayout.LabelField($"{shapeTags[i]}: {squareCount}", GUILayout.Width(75f), GUILayout.Height(20f));
            EditorGUILayout.EndHorizontal();
        }
        #endregion
        EditorGUILayout.Separator();

        #region Color Settings
        EditorGUILayout.LabelField("Color Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(150f));
        EditorGUILayout.Separator();

        string[] colorTags = Enum.GetNames(typeof(GameShape.ColorType));
        GameShape.ColorType currentColorType;
        for (int i = 0; i < shapeTags.Length; i++)
        {
            currentColorType = (GameShape.ColorType)i;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(20f), GUILayout.Height(20f)))
            {
                if (genShapePrefs.colors.TryGetValue(currentColorType, out int sCount))
                    genShapePrefs.colors[currentColorType] = Mathf.Clamp(sCount + 1, 0, sCount + genShapePrefs.GetRemainingColors(GetBoardSize()));
                else
                    genShapePrefs.colors.Add(currentColorType, Mathf.Clamp(1, 0, sCount + genShapePrefs.GetRemainingColors(GetBoardSize())));
            }

            if (GUILayout.Button("-", GUILayout.Width(20f), GUILayout.Height(20f)))
            {
                if (genShapePrefs.colors.TryGetValue(currentColorType, out int sCount))
                    genShapePrefs.colors[currentColorType] = Mathf.Clamp(sCount - 1, 0, sCount + genShapePrefs.GetRemainingColors(GetBoardSize()));
                else
                    genShapePrefs.colors.Add(currentColorType, 0);
            }

            int squareCount = genShapePrefs.colors.TryGetValue(currentColorType, out int squares) ? squares : 0;
            EditorGUILayout.LabelField($"{colorTags[i]}: {squareCount}", GUILayout.Width(75f), GUILayout.Height(20f));
            EditorGUILayout.EndHorizontal();
        }
        #endregion
        EditorGUILayout.Separator();

        if (genPrefsMismatchError)
            EditorGUILayout.LabelField("Ensure the selected shape count and color count match before generating a level", EditorStyles.helpBox);

        if (zeroShapeScaleError)
            EditorGUILayout.LabelField("Ensure the shape size is more that 0 before generating a level", EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Board", GUILayout.MaxWidth(200f)))
        {
            genPrefsMismatchError = (genShapePrefs.GetCurrentColorCount() != genShapePrefs.GetCurrentShapeCount());
            zeroShapeScaleError = (genShapePrefs.targetShapeScale <= 0f);

            if (!genPrefsMismatchError && !zeroShapeScaleError)
            {
                GameboardTools.ClearGameboard(true, true);
                LevelGenerator.GenerateGameboardShapes(genShapePrefs);
            }
        }

        if (GUILayout.Button("Clear Board", GUILayout.MaxWidth(150f)))
            GameboardTools.ClearGameboard(true, true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Recreate Gameboard"))
        {
            GameManager manager = GameObject.Find("LevelManager").GetComponent<GameManager>();
            LevelGenerator.RecreateBoard(manager.gameBoardParent);
        }

        if (GUILayout.Button("Recreate Solutionboard"))
        {
            GameManager manager = GameObject.Find("LevelManager").GetComponent<GameManager>();
            LevelGenerator.RecreateBoard(manager.solutionBoardParent);
        }
        EditorGUILayout.EndHorizontal();
    }
}
